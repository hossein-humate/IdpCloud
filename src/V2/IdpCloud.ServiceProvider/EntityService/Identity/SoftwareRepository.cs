using IdpCloud.Common;
using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Identity
{
    /// <inheritdoc />
    public class SoftwareRepository : ISoftwareRepository
    {
        private readonly EfCoreContext _efDbContext;
        
        /// <summary>
        /// Initialises an instance of <see cref="SoftwareRepository"/> And inject the dbContext.
        /// </summary>

        public SoftwareRepository(EfCoreContext databaseContext)
        {
            _efDbContext = databaseContext;
        }

        /// <inheritdoc />
        public async Task<Software> GetSoftwareById(string clientId, CancellationToken cancellationToken)
        {
            var software = await _efDbContext.Softwares
                                 .Include(s => s.JwtSetting)
                                 .Where(s => s.SoftwareId.ToString() == clientId.DecodeBase64())
                                 .FirstOrDefaultAsync(cancellationToken);

            return software;
        }
    }
}
