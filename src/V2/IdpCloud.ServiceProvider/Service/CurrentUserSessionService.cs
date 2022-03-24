using IdpCloud.Common;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.Sdk;
using IdpCloud.ServiceProvider.EntityService.SSO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace IdpCloud.ServiceProvider.Service
{
    /// <summary>
    /// The current UserSession Service is immutable, returns the 
    /// UserSession object 
    /// </summary>
    public class CurrentUserSessionService : ICurrentUserSessionService
    {

        private UserSession _userSession;
        private bool _isAdministrator;
        private readonly IUserSessionRepository _newUserSessionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initialises an instance of <see cref="CurrentUserSessionService"/> 
        /// </summary>
        public CurrentUserSessionService(IUserSessionRepository newUserSessionRepository,
            IHttpContextAccessor httpContextAccessor,
            JwtSecurityTokenHandler jwtSecurityTokenHandler,
            IConfiguration configuration)
        {
            _newUserSessionRepository = newUserSessionRepository;
            _httpContextAccessor = httpContextAccessor;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _configuration = configuration;
        }

        public UserSession UserSession
        {
            get
            {
                if (this._userSession == null)
                {
                    var headers = _httpContextAccessor?.HttpContext?.Request?.Headers;
                    var token = headers != null && headers.ContainsKey(ApiManager.HeaderSecurityToken)
                                        ? headers[ApiManager.HeaderSecurityToken].ToString()
                                        : null;

                    if (!string.IsNullOrEmpty(token))
                    {
                        var jwtSecurityToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
                        var userSessionId = jwtSecurityToken.Claims.Where(c => c.Type == "UserSessionId").FirstOrDefault();
                        if (userSessionId != null)
                        {
                            var currentUserSession = _newUserSessionRepository.GetUserSessionByIdAndStatus(userSessionId.Value.ToGuid(), Status.Active).GetAwaiter().GetResult();
                            _userSession = currentUserSession;
                        }
                    }
                }
                return _userSession;
            }
        }

        public bool IsAdministrator
        {
            get
            {
                if (this._userSession != null)
                {
                    _isAdministrator = _userSession.User
                                                    .UserRoles
                                                    .Any(ar => ar.RoleId == _configuration["Application:AdministratorRoleId"].ToGuid());
                }
                return _isAdministrator;
            }

        }
    }
}
