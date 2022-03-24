using AutoMapper;
using IdpCloud.REST.Infrastructure.Helper;
using IdpCloud.REST.Infrastructure.Middleware;
using IdpCloud.REST.Modules;
using IdpCloud.Common;
using IdpCloud.Common.Settings;
using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.BackgroundWorker;
using IdpCloud.ServiceProvider.InternalService.File;
using IdpCloud.ServiceProvider.InternalService.Mail;
using IdpCloud.ServiceProvider.Service;
using IdpCloud.ServiceProvider.Service.Identity;
using IdpCloud.ServiceProvider.Service.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace IdpCloud.REST
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AddRazorPages();
            services.AddControllers(option =>
                {
                    option.Conventions.Add(new ActionHidingConvention());
                })
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling =
                    ReferenceLoopHandling.Ignore);

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(3);
                options.ExpireTimeSpan = TimeSpan.FromDays(3);
                options.SlidingExpiration = true;
            });

            services.AddDbContext<EfCoreContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:Connection"],
                    b => b.MigrationsAssembly("IdpCloud.DataProvider")));

            services.AddHttpContextAccessor();
            services.AddUnitOfWork();

            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IResetPasswordService, ResetPasswordService>();
            services.AddScoped<IRateLimitService, RateLimitService>();
            services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();
            services.AddScoped<IApiAuthenticationService, ApiAuthenticationService>();
            services.AddScoped<IAwsMailServiceProvider, AwsMailServiceProvider>();
            services.AddScoped<IEmailService, AwsEmailService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserSoftwareService, UserSoftwareService>();
            services.AddScoped<IRandomPasswordGenerator, RandomPasswordGenerator>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<SendMailHostedService>();
            services.AddHostedService<QueuedHostedService>();
            services.AddHostedService<SessionTimedHostedService>();
            services.AddScoped<ICurrentUserSessionService, CurrentUserSessionService>();
            services.AddScoped<JwtSecurityTokenHandler>();
            services.AddScoped<ICurrentSoftwareService, CurrentSoftwareService>();
            services.AddScoped<IOrganisationService, OrganisationService>();
            services.AddLogging(builder => { builder.ClearProviders().AddConsole(); });
            services.AddResponseCaching(op => { op.MaximumBodySize = 2048; });
            services.AddOptions();
            services.Configure<GlobalParameterSetting>(Configuration.GetSection("GlobalParameter"));
            services.Configure<MailServiceSetting>(Configuration.GetSection("MailService"));
            services.Configure<AwsSetting>(Configuration.GetSection("Aws"));

            services.RegisterCorsPolicy(Configuration.GetSection("Application").Get<ApplicationSetting>().AllowedOrigins);

            //Prevent From Redirecting Server Loop back 
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("Identity", new OpenApiInfo
                {
                    Title = "Identity Api",
                    Version = "v1"
                });
                options.SwaggerDoc("BaseInfo", new OpenApiInfo
                {
                    Title = "Basic Information Api",
                    Version = "v1"
                });
                options.SwaggerDoc("SSO", new OpenApiInfo
                {
                    Title = "Single Sign On Api",
                    Version = "v1"
                });
                options.SwaggerDoc("Security", new OpenApiInfo
                {
                    Title = "Security Api",
                    Version = "v1"
                });
                options.CustomSchemaIds(x =>
                {
                    try
                    {
                        var parentName = x.Namespace?.Split(".")[^1];
                        return parentName + "." + x.Name;
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, EfCoreContext efCoreContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/Identity/swagger.json", "Identity Api v1");
                    options.SwaggerEndpoint("/swagger/BaseInfo/swagger.json", "Basic Information Api v1");
                    options.SwaggerEndpoint("/swagger/SSO/swagger.json", "Single Sign On Api v1");
                    options.SwaggerEndpoint("/swagger/Security/swagger.json", "Security Api v1");
                });
            }

            efCoreContext.Database.Migrate();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseCors("cors-policy");
            app.UseHsts();
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Context.Response.Headers.Add("Expires", "-1");
                    context.Context.Response.Headers.Add("Pragma", "no-cache, no-store");
                }
            });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCheckBanClients();
            app.UseApiAuthentication();
            app.Use(async (context, next) =>
            {
                context.Response.Headers.TryAdd("X-Frame-Options", "SAMEORIGIN");
                context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
                context.Response.Headers.TryAdd("Referrer-Policy", "strict-origin-when-cross-origin");
                context.Response.Headers.TryAdd("X-Permitted-Cross-Domain-Policies", "none");
                context.Response.Headers.TryAdd("Feature-Policy", "geolocation 'self'");
                context.Response.Headers.TryAdd("Feature-Policy", "geolocation 'self'");
                context.Response.Headers.TryAdd("X-Xss-Protection", "1");
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("Server");
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
