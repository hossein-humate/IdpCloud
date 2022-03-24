using IdpCloud.DataProvider.Entity.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Identity
{
    /// <summary>
    /// The Software repository
    /// </summary>
    public interface ISoftwareRepository
    {
        /// <summary>
        /// Get Software by ID/ClientId
        /// </summary>
        /// <param name="clientId">The clientId is encoded softwareId from Header </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>The<see cref="Software"/>Software entity</returns>
        Task<Software> GetSoftwareById(string clientId, CancellationToken cancellationToken);

    }
}
