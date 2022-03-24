using AutoMapper;
using IdpCloud.REST.Infrastructure.Controller;
using IdpCloud.Sdk.Model;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.REST.Areas.SSO.Structure
{
    [Area("SSO")]
    [ApiExplorerSettings(GroupName = "SSO")]
    public class SsoBaseController : ApiBaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public SsoBaseController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, ICurrentUserSessionService currentUserSession) :
            base(unitOfWork, configuration, mapper, currentUserSession)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpOptions("api/sso/IsValidToken")]
        public async Task<ActionResult<BaseResponse>> IsValidTokenAsync([FromServices]
            IJwtAuthenticationService authenticateService, CancellationToken cancellationToken = default)
        {
            try
            {
                //ToDO : Not using at the moment will fix it with proper arguments in next refactor task
                //return await authenticateService.CheckTokenValidateAsync(Request.Headers[ApiManager.HeaderSecurityToken],
                //    cancellationToken) ? Ok(new BaseResponse()) :
                //    StatusCode(403, BaseResponseCollection
                //        .GetBaseResponse(RequestResult.UserSessionInvalidOrExpired));

                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }
    }
}
