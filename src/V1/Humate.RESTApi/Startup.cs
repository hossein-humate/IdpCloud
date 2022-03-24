using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using DataProvider.DatabaseContext;
using EntityServiceProvider;
using EntityServiceProvider.EntityService.Identity;
using EntityServiceProvider.EntityService.Log;
using Humate.RESTApi.Infrastructure.Authentication;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.RESTApi.Infrastructure.InternalService.File;
using Humate.RESTApi.Infrastructure.InternalService.Mail;
using Humate.RESTApi.Infrastructure.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace Humate.RESTApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers()
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
                options.UseSqlServer(Configuration["ConnectionStrings:Connection"]));
            services.AddCors();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();
            services.AddScoped<IApiAuthenticationService, ApiAuthenticationService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            //DI Entities
            services.AddScoped<IVisitorRepository, VisitorRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IServerActivityRepository, ServerActivityRepository>();
            services.AddScoped<IAuthenticatedUser, AuthenticatedUser>();
            //services.AddScoped<AuthenticatedUser>();
            services.AddLogging(builder => { builder.ClearProviders().AddConsole(); });
            services.AddResponseCaching(op => { op.MaximumBodySize = 2048; });

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseCors(op => op.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

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

            //app.UseHttpsRedirection();

            app.UseRouting();

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

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/Identity/swagger.json", "Identity Api v1");
                options.SwaggerEndpoint("/swagger/BaseInfo/swagger.json", "Basic Information Api v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
