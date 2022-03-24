using IdpCloud.Sdk;
using IdpCloud.Sdk.Enum;
using IdpCloud.Sdk.Model;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IdpCloud.REST.Infrastructure.Middleware
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
            if (!await apiAuthenticationService.HasClientIdAsync())
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(
                    BaseResponseCollection.GetBaseResponse(RequestResult.ClientIdInvalidOrNull)));
                return;
            }

            // Allow Direct Calls to IdP Backend: Clients to API request calls
            if (AnonymousPath(context.Request))
            {
                await _next.Invoke(context);
                return;
            }

            var result = await jwtAuthenticateService.CheckTokenValidateAsync(context.Request.Headers[ApiManager.HeaderSecurityToken]);
            switch (result)
            {
                case TokenStatus.Valid:
                    await _next.Invoke(context);
                    break;
                case TokenStatus.HasRefreshTime:
                    await IsRefreshTokenPath(context);
                    break;
                case TokenStatus.Invalid:
                    await HandleUnAuthorisedRequest(context, RequestResult.UserSessionInvalidOrExpired);
                    break;
            }

        }

        private async Task IsRefreshTokenPath(HttpContext context)
        {
            if (context.Request.Path.ToString().Equals("/api/sso/userSession/RefreshToken",
                       StringComparison.OrdinalIgnoreCase))
            {
                await _next.Invoke(context);
                return;
            }
            else
            {
                await HandleUnAuthorisedRequest(context, RequestResult.LimitOnRefreshTime);
            }
        }

        private async Task HandleUnAuthorisedRequest(HttpContext context, RequestResult requestResult)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(
                BaseResponseCollection.GetBaseResponse(requestResult)));
        }

        // Todo - reflect the controller/action route
        private static readonly string[] Anonymous = {
            "/api/sso/Registration/Register",
            "/api/sso/Registration/SendEmailConfirmation",
            "/api/baseinfo/",
            "/api/sso/Login",
            "/api/sso/resetpassword/RequestByEmail",
            "/api/sso/resetpassword/ChangePassword",
            "/api/sso/resetpassword/Decline",
            "/api/sso/Registration/ConfirmEmail",
        };

        private static bool AnonymousPath(HttpRequest request)
        {
            return Anonymous.Any(path =>
                request.Path.ToString().StartsWith(path, StringComparison.OrdinalIgnoreCase));
        }
    }
}