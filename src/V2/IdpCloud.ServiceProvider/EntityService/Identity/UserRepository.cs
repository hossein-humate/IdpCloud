using IdpCloud.Common;
using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Identity
{
    /// <inheritdoc />
    public class UserRepository : IUserRepository
    {
        private readonly EfCoreContext _dbContext;
        /// <summary>
        /// Initialises an instance of <see cref="UserRepository"/> And inject the dbContext.
        /// </summary>
        public UserRepository(EfCoreContext efCoreContext)
        {
            _dbContext = efCoreContext;
        }

        public async Task<User> Add(User user)
        {
            return (await _dbContext.Users.AddAsync(user)).Entity;
        }

        public void Delete(User user)
        {
            user.DeleteDate = DateTime.UtcNow;
            _dbContext.Users.Update(user);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetAll(CancellationToken cancellationToken = default)
        {
            var users = await _dbContext.Users
              .Include(u => u.Language)
              .Include(u => u.UserSoftwares)
              .Select(u => new User
              {
                  UserId = u.UserId,
                  Username = u.Username,
                  Email = u.Email,
                  Status = u.Status,
                  CreateDate = u.CreateDate,
                  Language = u.Language,
                  LanguageId = u.LanguageId,
                  UserSoftwares = u.UserSoftwares.Where(us => us.DeleteDate == null).ToList()
              })
              .ToListAsync(cancellationToken);

            return users;
        }

        /// <inheritdoc />
        public async Task<User> GetByEmail(string email, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users
                            .Where(u => u.Email.ToLower() == email.ToLower())
                            .FirstOrDefaultAsync(cancellationToken);
            return user;
        }

        /// <inheritdoc />
        public async Task<User> GetUserByUserNameOrEmailAndPassword(string usernameOrEmail, string password, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users
                            .Where(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail)
                            .Include(u => u.Organisation)
                            .Include(u => u.UserRoles)
                                .ThenInclude(ur => ur.Role)
                            .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                return null;
            }

            return !password.ValidateHash(user.PasswordSalt, user.PasswordHash) ? null : user;
        }

        /// <inheritdoc />
        public User Update(User user)
        {
            user.UpdateDate = DateTime.UtcNow;
            _dbContext.Users.Update(user);
            return user;
        }

        public async Task<User> GetByUsername(string username, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u =>
                u.Username.ToLower() == username.ToLower(), cancellationToken);
        }

        /// <inheritdoc />
        public async Task<User> FindById(Guid? userId, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
            _dbContext.Entry(user).State = EntityState.Detached;
            return user;
        }

        ///<inheritdoc/>
        public async Task<User> GetByEmailConfirmationSecretAndId(Guid? userId, string emailConfirmationSecret, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.Where(u => u.UserId == userId && u.EmailConfirmationSecret == emailConfirmationSecret)
                                        .FirstOrDefaultAsync(cancellationToken);
            return user;
        }
    }
}
