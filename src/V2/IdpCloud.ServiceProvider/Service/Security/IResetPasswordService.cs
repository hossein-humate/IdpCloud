using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Security.Request.ResetPassword;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service.Security
{
    /// <summary>
    /// Service to provide business logic for Resetting Password
    /// </summary>
    public interface IResetPasswordService
    {
        /// <summary>
        /// Check the email address in the given request and provide a BaseResponse.
        /// </summary>
        /// <param name="email">containing the user's email address.</param>
        /// <param name="clientIp">The IP address from which the user attempted a request for reset password.</param>
        /// <param name="clientUserAgent">The User Agent (Browser) from which the user attempted a request for reset password.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation.</returns>
        Task RequestByEmailAsync(string email, string clientIp, string clientUserAgent,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check the Reset password Expiry, Active, Secret and ... . Replace the new password with the old one and provide a BaseResponse.
        /// </summary>
        /// <param name="request">containing the request parameters <see cref="ChangePasswordRequest"/>.</param>
        /// <param name="clientIp">The IP address from which the user attempted a request for reset password.</param>
        /// <param name="clientUserAgent">The User Agent (Browser) from which the user attempted a request for reset password.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>Return a <see cref="Task"/> representing an async operation that contain <see cref="bool"/> of this operation.</returns>
        Task<bool> ChangePasswordAsync(ChangePasswordRequest request, string clientIp, string clientUserAgent,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check the Reset password Expiry, Active, Secret and ... . If Request is valid to process add new 
        /// Security Activiy and update the related ResetPassword request with Inactive and ClientRejectRequestResetPassword.
        /// </summary>
        /// <param name="request">containing the request parameters <see cref="DeclineRequest"/>.</param>
        /// <param name="clientIp">The IP address from which the user attempted a request for reset password.</param>
        /// <param name="clientUserAgent">The User Agent (Browser) from which the user attempted a request for reset password.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>Return a <see cref="Task"/> representing an async operation.</returns>
        Task DeclineAsync(DeclineRequest request, string clientIp, string clientUserAgent,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check the old password hash and set the new password value to the authenticated user account already loaded in the <see cref="ICurrentUserSessionService"/>
        /// </summary>
        /// <param name="request">containing the request parameters <see cref="NewPasswordRequest"/>.</param>
        /// <param name="clientIp">The IP address from which the user attempted a request for change old password.</param>
        /// <param name="clientUserAgent">The User Agent (Browser) from which the user attempted a request for change the old password.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>Return a <see cref="Task"/> representing an async operation that contain <see cref="bool"/> of this operation.</returns>
        Task<bool> NewPasswordAsync(NewPasswordRequest request, string clientIp, string clientUserAgent,
            CancellationToken cancellationToken = default);
    }
}
