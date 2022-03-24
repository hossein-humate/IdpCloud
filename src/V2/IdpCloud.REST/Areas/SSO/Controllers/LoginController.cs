using AutoMapper;
using IdpCloud.REST.Areas.SSO.Structure;
using IdpCloud.Sdk.Auth;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.Sdk.Model.Identity.Response.User;
using IdpCloud.Sdk.Model.SSO;
using IdpCloud.Sdk.Model.SSO.Response;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace IdpCloud.REST.Areas.SSO.Controllers
{
    [Route("api/sso/[controller]")]
    [ApiController]
    public class LoginController : SsoBaseController
    {
        public const string HeaderRemoteIpAddress = "X-Proxy-Client-Remote-Ip-Address";
        private const string HeaderUserAgent = "User-Agent";
        private readonly ILoginService _loginService;
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Instantiates a new instance of <see cref="LoginController"/>.
        /// </summary>
        /// <param name="loginService">An instance of <see cref="ILoginService"/> to inject.</param>
        /// <param name="unitOfWork">An instance of <see cref="IUnitOfWork"/> to inject.</param>
        /// <param name="configuration">An instance of <see cref="IConfiguration"/> to inject.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/> to inject.</param>
        /// <param name="currentUserSession">An instance of <see cref="ICurrentUserSessionService"/> to inject.</param>
        /// <param name="httpContextAccessor">An instance of <see cref="IHttpContextAccessor"/> to inject.</param>
        public LoginController(
            ILoginService loginService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper,
            ICurrentUserSessionService currentUserSession,
            IHttpContextAccessor httpContextAccessor) :
                base(unitOfWork, configuration, mapper, currentUserSession)
        {
            _loginService = loginService;
            _contextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// POST /api/sso/Login
        /// End point to perform the login operation.
        /// </summary>
        /// <param name="request">A <see cref="LoginRequest"/> model containing the user's credentials and the language for their session.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel ascyn operations (optional).</param>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in an <see cref="ActionResult{LoginResponse}"/> in the case of success.</returns>        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]    
        public async Task<ActionResult<LoginResponse>> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            ActionResult<LoginResponse> result;

            try
            {
                var headers = _contextAccessor?.HttpContext?.Request?.Headers;
                var remoteIpAddress = _contextAccessor?.HttpContext?.Connection?.RemoteIpAddress;
                var clientIp = headers != null && headers.ContainsKey(HeaderRemoteIpAddress)
                    ? headers[HeaderRemoteIpAddress].ToString()
                    : remoteIpAddress?.ToString();
                var clientUserAgent = headers != null && headers.ContainsKey(HeaderUserAgent)
                    ? headers[HeaderUserAgent].ToString()
                    : null;

                var loginResponse = await _loginService.Login(request, clientIp, clientUserAgent, cancellationToken);
                if (loginResponse != null)
                {
                    result = loginResponse.ResultCode == RequestResult.EmailNotConfirmed ?
                            BadRequest(loginResponse) :
                            Ok(loginResponse);
                }
                else
                {
                    result = Unauthorized(BaseResponseCollection
                        .GetBaseResponse(RequestResult.EmailUsernameOrPasswordWrong));
                }
            }
            catch
            {
                // TODO - log this
                result = StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }

            return result;
        }

        /// <summary>
        /// Controller method to get all required information about the current user, user-session,
        /// user organisation and authenticated user role that will load in an object of type <see cref="Sdk.Auth.AuthUser"/>
        /// </summary>
        /// <returns>An <see cref="ActionResult{AuthUserResponse}"/> in the case of success with TValue response 
        /// type of <see cref="AuthUserResponse"/>.</returns>        
        [HttpOptions("AuthUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<AuthUserResponse> AuthUser()
        {
            try
            {
                return Ok(new AuthUserResponse
                {
                    AuthUser = new AuthUser
                    {
                        User = Mapper.Map<User>(CurrentUserSession.UserSession?.User),
                        CurrentSession = Mapper.Map<UserSession>(CurrentUserSession?.UserSession),
                        Organisation = Mapper.Map<Organisation>(CurrentUserSession?.UserSession?.User?.Organisation),
                        Role = Mapper.Map<Role>(CurrentUserSession.UserSession?.User?.UserRoles?.FirstOrDefault(ur => ur.DeleteDate == null)?.Role)
                    }
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }
    }
}
