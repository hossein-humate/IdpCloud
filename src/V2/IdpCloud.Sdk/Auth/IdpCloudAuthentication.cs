using Basemap.Identity.Sdk;
using IdpCloud.Sdk.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IdpCloud.Sdk.Auth
{
    public class IdpCloudAuthentication
    {
        private readonly RequestDelegate _next;
        private readonly AuthenticationOptions _options;
        private IAuthUser _authUser;

        public IdpCloudAuthentication(RequestDelegate next, AuthenticationOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(HttpContext context, IIdpCloudClient basemapIdentityClient,
            IAuthUser authUser)
        {
            _options.Before?.Invoke(context);

            if (IsAnonymousUrl(context.Request))
            {
                await _next.Invoke(context);
                return;
            }

            if (!AuthenticationHeaderValue.TryParse(context.Request.Headers[_options.AuthenticationHeader],
                out var headerValue))
            {
                var response = JsonConvert.SerializeObject(_options.AuthenticationFailed == null ?
                    BaseResponseCollection.GetBaseResponse(RequestResult.AuthenticationHeaderNotFound) :
                    _options.AuthenticationFailed.Invoke(BaseResponseCollection
                        .GetBaseResponse(RequestResult.AuthenticationHeaderNotFound)));
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync(response);
                return;
            }
            if (_options.HasSchema)
            {
                if (!_options.AuthenticationSchema.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    var response = JsonConvert.SerializeObject(_options.AuthenticationFailed == null ?
                        BaseResponseCollection.GetBaseResponse(RequestResult.AuthenticationSchemaNotFound) :
                        _options.AuthenticationFailed.Invoke(BaseResponseCollection
                            .GetBaseResponse(RequestResult.AuthenticationSchemaNotFound)));
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync(response);
                    return;
                }
            }
            var result = await basemapIdentityClient.AuthUserAsync(headerValue.Parameter);
            if (result == null)
            {
                var response = JsonConvert.SerializeObject(_options.AuthenticationFailed == null ?
                    BaseResponseCollection.GetBaseResponse(RequestResult.IdentityServiceUnavailable) :
                    _options.AuthenticationFailed.Invoke(BaseResponseCollection
                            .GetBaseResponse(RequestResult.IdentityServiceUnavailable)));
                context.Response.StatusCode = 503;
                await context.Response.WriteAsync(response);
                return;
            }
            if (result.ResultCode != 0)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(
                    _options.AuthenticationFailed == null ? result : _options
                        .AuthenticationFailed.Invoke(result)));
                return;
            }
            _authUser = authUser;
            _authUser.User = result.AuthUser.User;
            _authUser.CurrentSession = result.AuthUser.CurrentSession;
            await _next.Invoke(context);
            _options.After?.Invoke(context, _authUser);
        }


        private bool IsAnonymousUrl(HttpRequest request)
        {
            return _options.AnonymousUrls.Any(path =>
                request.Path.ToString().StartsWith(path, StringComparison.OrdinalIgnoreCase));
        }
    }

    public static class BasemapAuthenticationExtention
    {
        public static IApplicationBuilder UseBasemapAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IdpCloudAuthentication>();
        }

        public static IApplicationBuilder UseBasemapAuthentication(this IApplicationBuilder builder,
            Action<AuthenticationOptions> configureOptions)
        {
            var options = new AuthenticationOptions();
            configureOptions(options);
            return builder.UseMiddleware<IdpCloudAuthentication>(options);
        }

        public static void AddBasemapIdentity(this IServiceCollection services,
            Action<ClientOptions> configureOptions = default)
        {
            var options = new ClientOptions();
            configureOptions?.Invoke(options);
            //services.Configure(configureOptions).AddSingleton(options);
            //services.AddSingleton<IIdpCloudClient, IdpCloudClient>();
            services.AddTransient<ApiManager>();
            services.AddScoped<IAuthUser, AuthUser>();
        }
    }
}
