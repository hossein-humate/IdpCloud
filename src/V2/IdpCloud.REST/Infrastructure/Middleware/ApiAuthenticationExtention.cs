using Microsoft.AspNetCore.Builder;

namespace IdpCloud.REST.Infrastructure.Middleware
{
    public static class ApiAuthenticationExtention
    {
        public static IApplicationBuilder UseApiAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiAuthentication>();
        }
    }
}