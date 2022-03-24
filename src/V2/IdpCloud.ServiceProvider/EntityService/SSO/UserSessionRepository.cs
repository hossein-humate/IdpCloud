using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.SSO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.SSO
{
    /// <inheritdoc />
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly EfCoreContext _dbContext;
        /// <summary>
        /// Initialises an instance of <see cref="UserSessionRepository"/> And inject the dbContext.
        /// </summary>
        public UserSessionRepository(EfCoreContext databaseContext)

        {
            _dbContext = databaseContext;
        }

        /// <inheritdoc />
        public async Task<UserSession> GetUserSessionByIdAndStatus(Guid? userSessionId, Status status, CancellationToken cancellationToken = default)
        {
            //Todo : Query need optimisation taking time to execute
            var userSession = await _dbContext.UserSessions.AsNoTracking()
                                    .Where(us => us.UserSessionId == userSessionId && us.Status == status)
                                    .Include(us => us.User)
                                    .Include(us => us.Software)
                                    .Include(us => us.Software.JwtSetting)
                                    .Include(us => us.User)
                                    .Include(us => us.User.Organisation)
                                    .Include(us => us.User.Language)
                                    .Include(us => us.User.UserSoftwares)
                                    .ThenInclude(us => us.Software)
                                    .ThenInclude(us => us.JwtSetting)
                                    .Include(us => us.User.UserRoles)
                                    .ThenInclude(us => us.Role)
                                    .FirstOrDefaultAsync(cancellationToken);

            return userSession;
        }
        /// <inheritdoc />
        public async void DeactivateUserSession(Guid? userSessionId, CancellationToken cancellationToken = default)
        {
            var userSession = await _dbContext.UserSessions.FindAsync(new object[] { userSessionId }, cancellationToken);
            if (userSession != null)
            {
                userSession.Status = Status.DeActive;
                userSession.UpdateDate = DateTime.UtcNow;
            }
        }

        /// <inheritdoc />
        public async Task<UserSession> Add(UserSession userSession, CancellationToken cancellationToken = default)
        {
            userSession.CreateDate = DateTime.UtcNow;
            await _dbContext.AddAsync(userSession, cancellationToken);
            return userSession;
        }

        /// <inheritdoc />
        public UserSession Update(UserSession userSession)
        {
            userSession.UpdateDate = DateTime.UtcNow;
            _dbContext.UserSessions.Update(userSession);
            return userSession;
        }

        /// <inheritdoc />
        public async Task<UserSession> FindById(Guid? userSessionId, CancellationToken cancellationToken = default)
        {
            var userSession = await _dbContext.UserSessions.FindAsync(new object[] { userSessionId }, cancellationToken);
            return userSession;
        }
    }
}
