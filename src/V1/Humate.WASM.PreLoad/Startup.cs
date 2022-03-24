using AutoMapper;
using Blazored.LocalStorage;
using Humate.WASM.Common;
using Humate.WASM.Service;
using Humate.WASM.Shared.Component.Toast;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Humate.WASM.PreLoad
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
            GlobalVariable.ApiKey = Configuration["ApiKey"];
            GlobalVariable.ApiBaseAddress = Configuration["ApiBaseAddress"];
            services.AddRazorPages();
            services.AddAutoMapper(typeof(Startup));
            services.AddBlazoredLocalStorage();
            services.AddScoped<ScopeService>();
            services.AddScoped<Clipboard>();
            services.AddToast();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseStatusCodePages();
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
