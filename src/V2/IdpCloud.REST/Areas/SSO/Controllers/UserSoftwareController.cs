using AutoMapper;
using IdpCloud.REST.Areas.SSO.Structure;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.SSO.Response.UserSoftware;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using IdpCloud.ServiceProvider.Service.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.REST.Areas.SSO.Controllers
{
    /// <summary>
    /// Represent Controller methods and APIs for the <see cref="Entity.Identity.UserSoftware"/> entity 
    /// related to the SSO area API Gateway
    /// </summary>
    [Route("api/sso/[controller]")]
    public class UserSoftwareController : SsoBaseController
    {
        private readonly IUserSoftwareService _userSoftwareService;
        private readonly ICurrentUserSessionService _currentUserSession;

        /// <summary>
        /// Instantiates a new instance of <see cref="UserSoftwareController"/>.
        /// </summary>
        /// <param name="unitOfWork">An instance of <see cref="IUnitOfWork"/> to inject.</param>
        /// <param name="configuration">An instance of <see cref="IConfiguration"/> to inject.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/> to inject.</param>
        /// <param name="currentUserSession">An instance of <see cref="ICurrentUserSessionService"/> to inject.</param>
        public UserSoftwareController(IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper,
            ICurrentUserSessionService currentUserSessionService,
            IUserSoftwareService userSoftwareService) :
            base(unitOfWork, configuration, mapper, currentUserSessionService)
        {
            _userSoftwareService = userSoftwareService;
            _currentUserSession = currentUserSessionService;
        }

        /// <summary>
        /// Controller method for getting software list those are allowed to use for the current authenticated user
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel ascyn operations (optional).</param>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in an <see cref="ActionResult{SoftwareListResponse}"/> in the case of success.</returns>        
        [HttpGet("SoftwareList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SoftwareListResponse>> SoftwareListAsync(
            CancellationToken cancellationToken = default)
        {
            ActionResult<SoftwareListResponse> response;

            try
            {
                var softwares = await _userSoftwareService.SoftwareList(
                    _currentUserSession.UserSession.UserId,
                    cancellationToken);
                response = Ok(new SoftwareListResponse
                {
                    Softwares = Mapper.Map<IEnumerable<SoftwareDto>>(softwares)
                });
            }
            catch
            {
                response = StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }

            return response;
        }
    }
}
