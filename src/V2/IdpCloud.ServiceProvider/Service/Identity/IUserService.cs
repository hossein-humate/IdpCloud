using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.Sdk.Model.SSO.Request.User;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service.Identity
{
    /// <summary>
    /// Service to provide business logic for User Entity and User Controller
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Service method that would create a new User account after validate the request parameters,
        /// and send confirmation Email to the provided email address
        /// </summary>
        /// <param name="request">Represent register request</param>
        /// <param name="ip">Represent the client IP Address</param>
        /// <param name="userAgent">Represent the client User-Agent</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation that is contain <see cref="RequestResult"/></returns>
        Task<RequestResult> Register(
            RegisterRequest request,
            string ip,
            string userAgent,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Service method that would confirm email after validate the request parameters,
        /// and records into system and send confirmation Email to the provided email address
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="secret">Email confirmation sceret</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A < see cref="BaseResposne">Baseresponse collection</returns>
        Task<RequestResult> ConfirmEmail(
            string key,
            string secret,
            CancellationToken cancellationToken = default);


        /// <summary>
        /// Validates the given Resend request and provide a response containing the resultCode and message.
        /// </summary>
        /// <param name="request">The <see cref="SendEmailConfirmationRequest"/> containing the user's credentials.</param>
        /// <param name="clientIp">The IP address from which the user attempted a login.</param>
        /// <param name="clientUserAgent">The User Agent (Browser) from which the user attempted a login.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation, resulting in a <see cref="RequestResult"/> if the credentials in
        /// the <see cref="SendEmailConfirmationRequest"/> are valid, otherwise return the related response message.</returns>
        Task<RequestResult> SendEmailConfirmation(
            SendEmailConfirmationRequest request,
            string clientIp,
            string clientUserAgent,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Service method that would create a new User account after validate the request parameters,
        /// and send confirmation Email to the provided email address
        /// </summary>
        /// <param name="createUserRequest">Represents the Create User request</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation that contain <see cref="User"/> record created by this process</returns>
        Task<User> Create(
            CreateUserRequest createUserRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Service method to get existing users in the pagination result
        /// </summary>
        /// <param name="paginationParam">A <see cref="UserPaginationParam"/> param</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in
        /// an <see cref="UserPaginationResult"/></returns>
        Task<UserPaginationResult> GetAll(UserPaginationParam paginationParam, CancellationToken cancellationToken = default);


        /// <summary>
        /// Service method for editing the existing User by admin
        /// </summary>
        /// <param name="request">Represent DTO of user from <see cref="UpdateUserRequest"/> type</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation</returns>
        Task<User> Update(UpdateUserRequest request, CancellationToken cancellationToken = default);
    }
}
