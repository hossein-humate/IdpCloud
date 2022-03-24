using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.Sdk.Model.SSO.Response.UserSession;
using IdpCloud.ServiceProvider.EntityService.SSO;
using System;
using System.Threading;
using System.Threading.Tasks;
using IdpCloud.Common;
using IdpCloud.Sdk.Model.SSO.Request.UserSession;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk;
using Microsoft.AspNetCore.Http;

namespace IdpCloud.ServiceProvider.Service
{
    ///<inheritdoc/>
    public class RefreshTokenService : IRefreshTokenService
    {
        ///<inheritdoc/>
        
        private readonly ICurrentUserSessionService _currentUserSessionService;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IJwtAuthenticationService _jwtAuthenticationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        ///  Initialises an instance of <see cref="RefreshTokenService"/>.
        /// </summary>
        /// <param name="currentUserSessionService"><see cref="CurrentUserSessionService"/>Current User Session</param>
        /// <param name="userSessionRepository"><see cref="UserSessionRepository"/>User Session Repository</param>
        public RefreshTokenService(ICurrentUserSessionService currentUserSessionService,
            IUserSessionRepository userSessionRepository,
            IJwtAuthenticationService jwtAuthenticationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _currentUserSessionService = currentUserSessionService ?? throw new ArgumentNullException(nameof(currentUserSessionService));
            _userSessionRepository = userSessionRepository ?? throw new ArgumentNullException(nameof(userSessionRepository));
            _jwtAuthenticationService = jwtAuthenticationService ?? throw new ArgumentNullException(nameof(jwtAuthenticationService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(_httpContextAccessor));

        }
        public async Task<RefreshTokenResponse> CreateRefreshToken(RefreshTokenRequest refreshTokenRequest,
                CancellationToken cancellationToken= default)
        {
            var currentUserSession = _currentUserSessionService.UserSession;

            var result = Validate(currentUserSession, refreshTokenRequest);

            if (result != RequestResult.RequestSuccessful)
            {
                var baseResponse = BaseResponseCollection.GetBaseResponse(result);
                return new RefreshTokenResponse () {ResultCode = baseResponse.ResultCode, Message = baseResponse.Message};
            }
            var userSessionId = currentUserSession.UserSessionId;

            DeactivateUserSession(userSessionId, cancellationToken);

            var userSession = CreateUserSession(currentUserSession, refreshTokenRequest, cancellationToken);

            var refreshTokenResponse = new RefreshTokenResponse()
            {
                Token = await _jwtAuthenticationService.CreateTokenAuthenticationAsync(userSession.Result, cancellationToken),
                RefreshToken = Cryptography.GenerateRefreshToken() 
            };

            return refreshTokenResponse;
        }

        private async Task<UserSession> CreateUserSession(UserSession oldUserSession, RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
        {
            var userSession = new UserSession
            {
                AuthType = oldUserSession.AuthType,
                Status = Status.Active,
                Ip = refreshTokenRequest.ClientIp,
                UserAgent = refreshTokenRequest.ClientUserAgent,
                SoftwareId = oldUserSession.SoftwareId,
                UserId = oldUserSession.UserId
            };

            await _userSessionRepository.Add(userSession, cancellationToken);

            return userSession;
        }

        private void DeactivateUserSession(Guid userSessionId, CancellationToken  cancellationToken)
        {
             _userSessionRepository.DeactivateUserSession(userSessionId, cancellationToken);
        }

        private RequestResult Validate(UserSession userSession, RefreshTokenRequest  refreshTokenRequest)
        {
            RequestResult result = RequestResult.RequestSuccessful;

            if (!userSession.Software.JwtSetting.HasRefresh)
            {
                result = RequestResult.RefreshTokenIsDisable;
                return result;
            }
            if (!refreshTokenRequest.RefreshToken.Equals(userSession.RefreshToken))
            {
                result = RequestResult.InvalidRefreshToken;
                return result;
            }

            var headers = _httpContextAccessor?.HttpContext?.Request?.Headers;
            var token = headers != null && headers.ContainsKey(ApiManager.HeaderSecurityToken)
                                ? headers[ApiManager.HeaderSecurityToken].ToString()
                                : null;

            if (!string.IsNullOrEmpty(token))
            {
                var handler = _jwtAuthenticationService.GetJwtSecurityHandler(token, userSession);

                if (DateTime.UtcNow <= handler.SecurityToken.ValidTo)
                {
                    result = RequestResult.CannotRefreshBeforeExpiration;
                    return result;
                }
                if (DateTime.UtcNow > handler.SecurityToken.ValidTo.AddMonths(
                           userSession.Software.JwtSetting.RefreshExpireMinute))
                {
                    result = RequestResult.RefreshTokenHasBeenExpired;
                    return result;
                }
            }
            else
            {
                return RequestResult.NotAllowedThisOperation;
            }

            return result;
        }
    }  
}
