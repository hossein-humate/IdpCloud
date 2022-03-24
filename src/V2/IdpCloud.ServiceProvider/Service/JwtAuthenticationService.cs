using IdpCloud.Common;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.Sdk.Enum;
using IdpCloud.ServiceProvider.EntityService.SSO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service
{
    /// <inheritdoc />
    public class JwtAuthenticationService : IJwtAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserSessionService _currentUserSessionService;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly ICurrentSoftwareService _currentSoftwareService;

        public JwtAuthenticationService(IUnitOfWork unitOfWork,
            ICurrentUserSessionService currentUserSessionService,
            IUserSessionRepository userSessionRepository,
            ICurrentSoftwareService currentSoftwareService)
        {
            _unitOfWork = unitOfWork;
            _currentUserSessionService = currentUserSessionService;
            _userSessionRepository = userSessionRepository;
            _currentSoftwareService = currentSoftwareService;
        }

        /// <inheritdoc />
        public async Task<string> CreateTokenAuthenticationAsync(UserSession userSession,
            CancellationToken cancellationToken = default)
        {
            var currentSoftware = _currentSoftwareService.Software;

            IEnumerable<Claim> claims = new List<Claim>
                {
                    new Claim("UserSessionId", userSession.UserSessionId.ToString())
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(currentSoftware.JwtSetting.Secret));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(issuer: currentSoftware.JwtSetting.Issuer,
                audience: currentSoftware.JwtSetting.Audience, 
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(currentSoftware.JwtSetting.ExpireMinute),
                signingCredentials: credentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        /// <inheritdoc />
        public async Task<TokenStatus> CheckTokenValidateAsync(string token,
            CancellationToken cancellationToken = default)
        {

            var currentUserSession = _currentUserSessionService.UserSession;

            if (currentUserSession == null)
            {
                return TokenStatus.Invalid;
            }
            var handler = GetJwtSecurityHandler(token, currentUserSession);
            if (handler == null)
            {
                return TokenStatus.Invalid;
            }
            var validTo = handler.SecurityToken.ValidTo;
            var userSessionId = handler.Claims.FirstOrDefault(c => c.Type == "UserSessionId")?.Value.ToGuid();

            if (DateTime.UtcNow > validTo)
            {
                if (currentUserSession.Software.JwtSetting.HasRefresh && DateTime.UtcNow <=
                    validTo.AddMinutes(currentUserSession.Software.JwtSetting.RefreshExpireMinute))
                {
                    return TokenStatus.HasRefreshTime;
                }
                //IF token and RefreshToken both expired Deactive UserSession
                await DeactivateUserSessionAsync(userSessionId, cancellationToken);
                return TokenStatus.Invalid;
            }
            return TokenStatus.Valid;

        }

        /// <inheritdoc />
        public JwtSecurityHandlerResponse GetJwtSecurityHandler(string token, UserSession userSession)
        {
            var param = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(userSession.Software.JwtSetting.Secret)),
                ValidateLifetime = false,
                RequireExpirationTime = true,
                ValidIssuer = userSession.Software.JwtSetting.Issuer,
                ValidAudience = userSession.Software.JwtSetting.Audience,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero,
                ValidateAudience = false
            };
            try
            {
                var claims = new JwtSecurityTokenHandler()
                            .ValidateToken(token, param, out var securityToken);

                return new JwtSecurityHandlerResponse
                {
                    SecurityToken = securityToken,
                    Claims = claims.Claims
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task DeactivateUserSessionAsync(Guid? userSessionId, CancellationToken cancellationToken = default)
        {
            _userSessionRepository.DeactivateUserSession(userSessionId, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

    }
}
