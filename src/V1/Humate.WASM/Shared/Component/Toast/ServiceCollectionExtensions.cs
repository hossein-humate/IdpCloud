using Microsoft.Extensions.DependencyInjection;

namespace Humate.WASM.Shared.Component.Toast
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddToast(this IServiceCollection services)
        {
            return services.AddScoped<IToastService, ToastService>();
        }
    }
}
