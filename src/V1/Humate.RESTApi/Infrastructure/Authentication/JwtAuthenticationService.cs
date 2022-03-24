using EntityServiceProvider;
using General;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Entity.SSO;
using static System.String;
using Humate.RESTApi.Infrastructure.Authentication.Model;

namespace Humate.RESTApi.Infrastructure.Authentication
{
    public interface IJwtAuthenticationService
    {
        string CreateTokenAuthentication(Guid userSessionId);

        Task<string> CheckTokenValidateAsync(string token,
            CancellationToken cancellationToken = default);

        Task<bool> CheckTokenExpiredAsync(string token, CancellationToken cancellationToken = default);
    }

    public class JwtAuthenticationService : IJwtAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticatedUser _authenticatedUser;
        public JwtAuthenticationService(IConfiguration configuration, IUnitOfWork unitOfWork,
            IAuthenticatedUser authenticatedUser)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _authenticatedUser = authenticatedUser;
        }

        public string CreateTokenAuthentication(Guid userSessionId)
        {
            try
            {
                IEnumerable<Claim> claims = new List<Claim>
                {
                    new Claim("UserSessionId", userSessionId.ToString())
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Secret"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwtToken = new JwtSecurityToken(
                    _configuration["Token:Issuer"],
                    _configuration["Token:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Token:AccessExpiration"])),
                    signingCredentials: credentials
                );
                return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            }
            catch (Exception)
            {
                return Empty;
            }
        }

        public async Task<string> CheckTokenValidateAsync(string token,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var param = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Secret"])),
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidIssuer = _configuration["Token:Issuer"],
                    ValidAudience = _configuration["Token:Audience"],
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, param, out _);
                var validTo = handler.ReadToken(token).ValidTo;
                if (DateTime.UtcNow > validTo)
                {
                    await DeactivateUserSessionAsync(handler.ReadJwtToken(token).Claims.FirstOrDefault(c =>
                        c.Type == "UserSessionId")?.Value.ToGuid(), cancellationToken);
                    return Empty;
                }

                var userSessionId = handler.ReadJwtToken(token).Claims.FirstOrDefault(c =>
                    c.Type == "UserSessionId")?.Value;
                _authenticatedUser.UserSession = await _unitOfWork.UserSessions.FindAsNoTrackingAsync(u =>
                    u.UserSessionId == userSessionId.ToGuid(), cancellationToken, u => u.User, u => u.Software);
                if (_authenticatedUser.UserSession == null)
                {
                    return Empty;
                }

                _authenticatedUser.User = _authenticatedUser.UserSession.User;
                _authenticatedUser.Software = _authenticatedUser.UserSession.Software;
                _authenticatedUser.AccessRoles = (await _unitOfWork.UserRoles.FindAllAsNoTrackingAsync(ur =>
                    ur.UserId == _authenticatedUser.User.UserId, cancellationToken, ur => ur.Role)).Select(ur => ur.Role);

                if (_authenticatedUser.AccessRoles.Any(ar =>
                    ar.RoleId == _configuration["Application:AdministratorRoleId"].ToGuid())
                    && _authenticatedUser.Software.SoftwareId == _configuration["Application:SoftwareId"].ToGuid())
                {
                    _authenticatedUser.AccessSoftwares = await _unitOfWork.Softwares.FindAllAsNoTrackingAsync(s =>
                       true, cancellationToken);
                    _authenticatedUser.AccessRoles = await _unitOfWork.Roles.FindAllAsNoTrackingAsync(s =>
                        true, cancellationToken);
                }
                //Is not admin of Humate and is authenticated for Humate Dashboard
                else if (_authenticatedUser.Software.SoftwareId == _configuration["Application:SoftwareId"].ToGuid())
                {
                    _authenticatedUser.AccessSoftwares = (await _unitOfWork.UserSoftwares.FindAllAsNoTrackingAsync(us =>
                        us.UserId == _authenticatedUser.User.UserId && us.SoftwareId != _configuration["Application:SoftwareId"].ToGuid(), cancellationToken, us => us.Software))
                        .Select(us => us.Software);
                }
                else // For Other Software Set AccessRoles To Specific Software
                {
                    _authenticatedUser.AccessRoles = (await _unitOfWork.UserRoles.FindAllAsNoTrackingAsync(ur =>
                            ur.UserId == _authenticatedUser.User.UserId &&
                            ur.Role.SoftwareId == _authenticatedUser.Software.SoftwareId, cancellationToken,
                            ur => ur.Role))
                        .Select(ur => ur.Role);
                }
                //TODO Get User Permission By Current Login Role
                return token;
            }
            catch (Exception)
            {
                return Empty;
            }
        }

        public async Task<bool> CheckTokenExpiredAsync(string token, CancellationToken cancellationToken = default)
        {
            try
            {
                var param = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Secret"])),
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidIssuer = _configuration["Token:Issuer"],
                    ValidAudience = _configuration["Token:Audience"],
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var handler = new JwtSecurityTokenHandler();
                handler.ValidateToken(token, param, out _);
                var validTo = handler.ReadToken(token).ValidTo;
                if (DateTime.UtcNow <= validTo) return true;
                await DeactivateUserSessionAsync(handler.ReadJwtToken(token).Claims.FirstOrDefault(c =>
                    c.Type == "UserSessionId")?.Value.ToGuid(), cancellationToken);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Methods
        private async Task DeactivateUserSessionAsync(Guid? userSessionId, CancellationToken cancellationToken = default)
        {
            if (userSessionId == Guid.Empty) return;
            var userSession = await _unitOfWork.UserSessions.FindAsync(u =>
                u.UserSessionId == userSessionId, cancellationToken, u => u.User, u => u.Software);
            if (userSession == null) return;
            userSession.Status = Status.DeActive;
            userSession.UpdateDate = DateTime.Now.ConvertToTimestamp();
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        #endregion
    }
}
