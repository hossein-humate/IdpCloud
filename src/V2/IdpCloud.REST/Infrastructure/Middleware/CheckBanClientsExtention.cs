using Microsoft.AspNetCore.Builder;

namespace IdpCloud.REST.Infrastructure.Middleware
{
    /// <summary>
    /// Extension class to inject checking Ban Clients functionality to the API Gateway pipeline
    /// </summary>
    public static class CheckBanClientsExtention
    {
        /// <summary>
        /// Activate the CheckBanList middleware for all routes on top of the IdP API Gateway
        /// </summary>
        /// <param name="builder">input value of the current instance of hosted web api</param>
        /// <returns>Return the <see cref="IApplicationBuilder"/> that is currently have the CheckBanList functionality in its pipeline</returns>
        public static IApplicationBuilder UseCheckBanClients(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CheckBanClients>();
        }
    }
}
