using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.Sdk.Enum;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service
{
    /// <summary>
    /// Create JWT token and Validate it.
    /// </summary>
    public interface IJwtAuthenticationService
    {

        /// <summary>
        /// Create a JWT Token from 
        /// </summary>
        /// <param name="userSession">UserSession</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A <see cref="Task<string>" /> representing an async operation resulting in 
        /// return a string Token.</returns>
        Task<string> CreateTokenAuthenticationAsync(UserSession userSession,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks token is validate or not
        /// </summary>
        /// <param name="token">A JWT token string</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>A <see cref="Task<string>" /> representing an async operation resulting in 
        /// return a Token status <see cref="TokenStatus"/></returns>
        Task<TokenStatus> CheckTokenValidateAsync(string token,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Extractor the claims and securityToken from token and validate
        /// </summary>
        /// <param name="token">A JWT string token</param>
        /// <param name="userSession">Current userSession </param>
        /// <returns><see cref="Tuple<SecurityToken, IEnumerable<Claim>">
        /// Returns <see cref="JwtSecurityHandlerResponse"/>JwtSecurityHandlerResponse
        /// </returns>
        JwtSecurityHandlerResponse GetJwtSecurityHandler(string token, UserSession userSession);
    }
}
