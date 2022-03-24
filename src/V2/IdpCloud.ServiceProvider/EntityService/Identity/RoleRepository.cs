using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Identity
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        protected readonly DbSet<UserRole> _userRoles;

        /// <summary>
        /// Initialises an instance of <see cref="RoleRepository"/> And inject the dbContext.
        /// </summary>
        public RoleRepository(EfCoreContext efCoreContext)
        {
            _userRoles = efCoreContext.Set<UserRole>();
        }

        ///<inheritdoc/>
        public async Task<UserRole> AddUserRole(UserRole userRole)
        {
            return (await _userRoles.AddAsync(userRole)).Entity;
        }

        ///<inheritdoc/>
        public async Task<UserRole> FindByUserId(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _userRoles.Where(ur => 
            ur.UserId == userId &&
            ur.DeleteDate == null)
           .SingleOrDefaultAsync(cancellationToken);
        }

        ///<inheritdoc/>
        public void DeleteUserRole(UserRole userRole)
        {
            userRole.DeleteDate = DateTime.UtcNow;
            _userRoles.Update(userRole);
        }
    }
}
