using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service
{
    /// <summary>
    /// Checks if has valid clientId/software
    /// </summary>
    public interface IApiAuthenticationService
    {
        /// <summary>
        /// Checks if has valid Software
        /// </summary>
        /// <param name="cancellationToken"> used to cancel ascyn operations (optional)</param>
        /// <returns><see cref="Task<bool>"/>Return true if software exists else returns false</returns>
        Task<bool> HasClientIdAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Validate the software api key and checks for expiry
        /// </summary>
        /// <param name="apiKey">Software api key</param>
        /// <returns><see cref="Task<bool>"/>Return true if software api key is exits and valid else 
        /// returns false for invalid api key/ expiry</returns>
        bool ValidateSecretKey(string apiKey = default);
    }
}
