using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.Sdk.Model.Identity.Response.User;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service
{
    /// <summary>
    /// Serive to prodive business logic for login operations
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Validates the given login request and provide a response containing the user, session and jwt token.
        /// </summary>
        /// <param name="request">The <see cref="LoginRequest"/> containing the user's credentials.</param>
        /// <param name="clientIp">The IP address from which the user attempted a login.</param>
        /// <param name="clientUserAgent">The User Agent (Browser) from which the user attempted a login.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation, resulting in a <see cref="LoginResponse"/> if the credentials in
        /// the <see cref="LoginRequest"/> are valid, otherwise null.</returns>
        Task<LoginResponse> Login(LoginRequest request, string clientIp, string clientUserAgent, CancellationToken cancellationToken = default);
    }
}
