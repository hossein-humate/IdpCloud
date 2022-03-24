using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Security;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Security
{
    /// <inheritdoc/>
    internal class BanListRepository : IBanListRepository
    {
        protected readonly DbSet<BanList> _banLists;

        public BanListRepository(EfCoreContext databaseContext)
        {
            _banLists = databaseContext.Set<BanList>();
        }

        public async Task<BanList> AddAsync(BanList banList, CancellationToken cancellationToken = default)
        {
            return (await _banLists.AddAsync(banList, cancellationToken)).Entity;
        }

        public async Task<BanList> GetBannedIPAsync(string ip, CancellationToken cancellationToken = default)
        {
            return await _banLists.SingleOrDefaultAsync(b => ip.Equals(b.Ip) && b.ReleaseDate >= System.DateTime.UtcNow, cancellationToken);
        }

        public BanList Update(BanList banList)
        {
            return _banLists.Update(banList).Entity;
        }
    }
}
