using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using IdpCloud.REST.Areas.SSO.Structure;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.SSO.Request.UserSession;
using IdpCloud.Sdk.Model.SSO.Response.UserSession;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IdpCloud.REST.Areas.SSO.Controllers
{
    /// <summary>
    /// Represent Controller methods and APIs for the <see cref="Entity.SSO.UserSession"/> entity
    /// </summary>
    
    [Route("api/sso/[controller]")]
    public class UserSessionController : SsoBaseController
    {
        private readonly IRefreshTokenService _refreshTokenService;

        /// <summary>
        /// Instantiates a new instance of <see cref="UserSessionController"/>.
        /// </summary>
        /// <param name="unitOfWork">An instance of <see cref="IUnitOfWork"/> to inject.</param>
        /// <param name="configuration">An instance of <see cref="IConfiguration"/> to inject.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/> to inject.</param>
        /// <param name="currentUserSession">An instance of <see cref="ICurrentUserSessionService"/> to inject.</param>
        /// <param name="refreshTokenService">An instance of <see cref="IRefreshTokenService"/> to inject.</param>
        public UserSessionController(IUnitOfWork unitOfWork, 
            IConfiguration configuration,
            IMapper mapper, 
            ICurrentUserSessionService currentUserSessionService,
            IRefreshTokenService  refreshTokenService) :
            base(unitOfWork, configuration, mapper, currentUserSessionService)
        {
            _refreshTokenService = refreshTokenService;
        }

        /// <summary>
        /// POST /api/identity/RefreshToken
        /// End point to create Refresh Token.
        /// </summary>
        /// <param name="request">A <see cref="RefreshTokenRequest"/> model containing the refresh Token, ClientIp and UserAgent.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel ascyn operations (optional).</param>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in an <see cref="ActionResult{RefreshTokenResponse}"/> 
        /// in the case of success.</returns> 
        [HttpPost("RefreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshTokenAsync(
            [FromBody] RefreshTokenRequest request, 
            CancellationToken cancellationToken = default)
        {
            ActionResult<RefreshTokenResponse> result;
            try
            {
                var refreshTokenResponse = await _refreshTokenService.CreateRefreshToken(request, cancellationToken);
                result = Ok(refreshTokenResponse);
            }
            catch (Exception)
            {
                // TODO - log this
                result = StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }
            return result;
        }
    }
}
