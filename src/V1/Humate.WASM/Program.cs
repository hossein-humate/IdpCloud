using AutoMapper;
using Blazored.LocalStorage;
using Humate.WASM.Service;
using Humate.WASM.Shared.Component.Toast;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Humate.WASM.Common;

namespace Humate.WASM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            GlobalVariable.ApiKey = builder.Configuration["ApiKey"];
            GlobalVariable.ApiBaseAddress = builder.Configuration["ApiBaseAddress"];
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAutoMapper(typeof(App));
            builder.Services.AddScoped<ScopeService>();
            builder.Services.AddScoped<Clipboard>();
            builder.Services.AddToast();
            await builder.Build().RunAsync();
        }
    }
}
