using AutoMapper;
using DataProvider.DatabaseContext;
using Entity.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using General;

namespace EntityServiceProvider.EntityService.Identity
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper) { }

        public async Task<User> IsValidUserAsync(string usernameOrEmail, string password,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await FindAsync(u =>
                    u.Username == usernameOrEmail || u.Email == usernameOrEmail, cancellationToken);
                if (user == null)
                {
                    return null;
                }

                return !password.ValidateHash(user.PasswordSalt, user.PasswordHash) ? null : user;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public interface IUserRepository : IRepository<User>
    {
        Task<User> IsValidUserAsync(string usernameOrEmail, string password,
            CancellationToken cancellationToken = default);
    }
}
