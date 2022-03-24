using IdpCloud.DataProvider.Entity.SSO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.SSO
{
    /// <summary>
    /// The UserSession repository
    /// </summary>
    public interface IUserSessionRepository
    {
        /// <summary>
        /// Get UserSession by ID and status
        /// </summary>
        /// <param name="userSessionId">The <see cref="UserSession.UserSessionId"/>UsersessionId</param>
        /// <param name="status">The <see cref="Status.Status"/>Status</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>The<see cref="UserSession"/>UserSession entity</returns>
        Task<UserSession> GetUserSessionByIdAndStatus(Guid? userSessionId, Status status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update UserSession object
        /// </summary>
        /// <param name="userSession">The <see cref="UserSession"/>UserSession</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        void DeactivateUserSession(Guid? userSessionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add new userSession
        /// </summary>
        /// <param name="userSession">Usersession</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        Task<UserSession> Add(UserSession userSession, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update UserSession object
        /// </summary>
        /// <param name="userSession">UserSession</param>
        UserSession Update(UserSession userSession);

        /// <summary>
        /// Find the Usersession by UserSEssionID
        /// </summary>
        /// <param name="userSessionId">The <see cref="UserSession.UserSessionId"/>UsersessionId</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns></returns>
        Task<UserSession> FindById(Guid? userSessionId, CancellationToken cancellationToken = default);
    }
}
