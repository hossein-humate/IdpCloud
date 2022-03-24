using Humate.RESTApi.Infrastructure.Authentication;
using Humate.Sdk.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.String;

namespace Humate.RESTApi.Infrastructure.Middleware
{
    public class ApiAuthentication
    {
        private readonly RequestDelegate _next;

        public ApiAuthentication(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IJwtAuthenticationService jwtAuthenticateService,
            IApiAuthenticationService apiAuthenticationService)
        {
            if (IsInExcludedUrls(context.Request) || context.Request.Path == "/")
            {
                if (await apiAuthenticationService
                    .ValidateSecretKeyAsync(context.Request.Headers["X-Secret-Key"]))
                {
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(
                        BaseResponseCollection.GetBaseResponse(RequestResult.InvalidApiKey)));
                }
            }
            else
            {
                if (!IsNullOrWhiteSpace(await jwtAuthenticateService
                    .CheckTokenValidateAsync(context.Request.Headers["X-Security-Token"])))
                {
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(
                        BaseResponseCollection.GetBaseResponse(RequestResult.UserSessionInvalidOrExpired)));
                }
            }
        }

        private static readonly string[] ExcludedList = {
            "/api/identity/user/AvailableUsername",
            "/api/identity/user/RegisterAndLogin",
            "/api/identity/user/LoginUser",
            "/api/identity/IsValidToken",
            "/api/identity/user/SuggestUsername",
            "/api/identity/user/GenerateJwt"
        };

        private static bool IsInExcludedUrls(HttpRequest request)
        {
            return ExcludedList.Any(path =>
                request.Path.ToString().StartsWith(path, StringComparison.OrdinalIgnoreCase));
        }
    }

    public static class ApiAuthenticationExtention
    {
        public static IApplicationBuilder UseApiAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiAuthentication>();
        }
    }
}