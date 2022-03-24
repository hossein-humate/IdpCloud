using IdpCloud.DataProvider.Entity.Enum;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service.Security
{
    /// <summary>
    /// Provide functionalities that is required for checking security concerns(exactly: Brute Force Attacks) on top of the IdP API Gateway policies
    /// </summary>
    public interface IRateLimitService
    {
        /// <summary>
        /// Check if the client IP address repeated same activity more than usual then it will put this
        /// Ip Address in the BanList with defualt release time +24 hours.
        /// </summary>
        /// <param name="ip">Represent the IP Address of client to check</param>
        /// <param name="type">Represent the limit count for any Security Activity that should be checked</param>
        /// <param name="limitCount">Represent the limit count for any process </param>
        /// <param name="releaseByhours">Represent the Release Date and Time if the client try to pass throw security policies</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>Return a <see cref="Task"/> that is contain <see cref="bool"/> result, it will be true if the client with same IP calling specific activity more than provided Limit Count in method inputs times</returns>
        Task<bool> CheckOnRateLimitAsync(string ip, ActivityType type, int limitCount = 5, int releaseByhours = 24, CancellationToken cancellationToken = default);
    }
}
