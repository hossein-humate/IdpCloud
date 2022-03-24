using IdpCloud.Sdk.Model.SSO.Request.UserSession;
using IdpCloud.Sdk.Model.SSO.Response.UserSession;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service
{
    /// <summary>
    /// Serive to prodive business logic for Refresh token
    /// </summary>
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Create new RefreshToken 
        /// </summary>
        /// <param name="refreshTokenRequest">The <see cref="RefreshTokenRequest"/> containing the refresh token, clientIp and UserAgent.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns><see cref="Task<RefreshTokenResponse>"/>Returns refresh token response conytaining valid JWT token & refresh token.</returns>
        Task<RefreshTokenResponse> CreateRefreshToken(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default);
    }
}
