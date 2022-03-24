using IdpCloud.Sdk.Model;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.EntityService.Security;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace IdpCloud.REST.Infrastructure.Middleware
{
    /// <summary>
    /// Resolve the client Request IP Address, if any IP is currently Banned and does not reach the ReleaseDate
    /// the reqeust pipeline would be finished, the ReleaseDate would be Refresh by +24 hours and response with Forbidden status code - 403 
    /// </summary>
    public class CheckBanClients
    {
        private readonly RequestDelegate _next;
        private const string HeaderRemoteIpAddress = "X-Proxy-Client-Remote-Ip-Address";

        public CheckBanClients(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork,IBanListRepository banListRepository)
        {
            var clientIp = context.Request.Headers.ContainsKey(HeaderRemoteIpAddress)
                  ? context.Request.Headers[HeaderRemoteIpAddress].ToString()
                  : context.Connection.RemoteIpAddress.ToString();
            var ban = await banListRepository.GetBannedIPAsync(clientIp);
            if (ban != null)
            {
                ban.ReleaseDate = DateTime.UtcNow.AddHours(24);
                banListRepository.Update(ban);
                await unitOfWork.CompleteAsync();
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(BaseResponseCollection
                    .GetBaseResponse(RequestResult.TooManyRequests)));
                return;
            }
            await _next.Invoke(context);
        }
    }
}
