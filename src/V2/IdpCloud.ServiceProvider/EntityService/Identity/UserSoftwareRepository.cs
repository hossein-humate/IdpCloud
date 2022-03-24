using IdpCloud.Common.Settings;
using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Identity
{
    /// <inheritdoc/>
    public class UserSoftwareRepository : IUserSoftwareRepository
    {
        private readonly EfCoreContext _dbContext;
        /// <summary>
        /// Initialises an instance of <see cref="UserSoftwareRepository"/> And inject the dbContext.
        /// </summary>
        public UserSoftwareRepository(EfCoreContext efCoreContext)
        {
            _dbContext = efCoreContext;
        }

        public async Task<IEnumerable<Software>> GetSoftwareListByUserId(Guid userId, CancellationToken cancellationToken = default)
        {
            var softwares = await _dbContext.UserSoftwares.Where(us =>
                    us.UserId == userId
                    && us.Software.Status == SoftwareStatus.Active
                    && us.Software.DeleteDate == null
                    && us.DeleteDate == null)
                .Include(us => us.Software)
                .ThenInclude(s => s.SoftwareDetail)
                .Select(us => us.Software)
                .ToListAsync(cancellationToken);
            return softwares;
        }
    }
}
