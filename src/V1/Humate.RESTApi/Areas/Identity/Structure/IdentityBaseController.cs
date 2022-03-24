using AutoMapper;
using EntityServiceProvider;
using Humate.RESTApi.Infrastructure.Authentication;
using Humate.RESTApi.Infrastructure.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;

namespace Humate.RESTApi.Areas.Identity.Structure
{
    [Area("Identity")]
    [ApiExplorerSettings(GroupName = "Identity")]
    public class IdentityBaseController : ApiBaseController
    {
        public IdentityBaseController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        {
        }

        [HttpOptions("api/identity/IsValidToken")]
        public async Task<ActionResult<BaseResponse>> IsValidTokenAsync(string token,
            [FromServices] IJwtAuthenticationService authenticateService, CancellationToken cancellationToken = default)
        {
            try
            {
                return await authenticateService.CheckTokenExpiredAsync(token, cancellationToken) ? Ok(new BaseResponse()) :
                    StatusCode(403, BaseResponseCollection
                        .GetBaseResponse(RequestResult.UserSessionInvalidOrExpired));
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }
    }
}
