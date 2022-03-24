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
    public class JwtSettingRepository : IJwtSettingRepository
    {
        private readonly EfCoreContext _efDbContext;

        /// <summary>
        /// Initialises an instance of <see cref="JwtSettingRepository"/> And inject the dbContext.
        /// </summary>

        public JwtSettingRepository(EfCoreContext databaseContext)
        {
            _efDbContext = databaseContext;
        }
        public async Task<JwtSetting> GetBySoftwareId(Guid SoftwareId, CancellationToken cancellationToken)
        {
            var jwtSetting = await _efDbContext.JwtSettings
                                 .Where(jwt => jwt.SoftwareId == SoftwareId)
                                 .FirstOrDefaultAsync(cancellationToken);

            return jwtSetting;
        }
    }
}
