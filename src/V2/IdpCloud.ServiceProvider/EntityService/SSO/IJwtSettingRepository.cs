using IdpCloud.DataProvider.Entity.SSO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.SSO
{
    /// <summary>
    /// The JwtSetting repository
    /// </summary>
    public interface IJwtSettingRepository
    {
        /// <summary>
        /// Get JwtSetting by SoftwareId
        /// </summary>
        /// <param name="softwareId">The SoftwareId </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>The<see cref="JwtSetting"/>JwtSetting entity</returns>
        Task<JwtSetting> GetBySoftwareId(Guid softwareId, CancellationToken cancellationToken = default);
    }
}
