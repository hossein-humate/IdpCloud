using Microsoft.Extensions.DependencyInjection;

namespace IdpCloud.REST.Modules
{
    public static class CorsPolicyIocExtensions
    {
        public static void RegisterCorsPolicy(this IServiceCollection services, string[] allowOrigins)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("cors-policy", builder =>
                {
                    builder.WithOrigins(allowOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
                });
            }
            );
        }
    }
}
