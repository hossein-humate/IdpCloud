using IdpCloud.DataProvider.Entity.Enum;
using IdpCloud.DataProvider.Entity.Security;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Security
{
    /// <summary>
    /// Service to provide Data Logics for Activity Entity
    /// </summary>
    public interface IActivityRepository
    {
        /// <summary>
        /// Add new <see cref="Activity"/> record to the database
        /// </summary>
        /// <param name="activity">Represent the record parameters data</param>
        /// <param name="cancellationToken">Cancel the operation if triggered</param>
        /// <returns>Return a Task <see cref="Task"/> that is contain the exact <see cref="Activity"/> record currently saved in database</returns>
        Task<Activity> AddAsync(Activity activity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Search for specific IP Address in Activiy Table and counts how many record created in 1 last hour
        /// </summary>
        /// <param name="ip">Represent the client IP Address</param>
        /// <param name="type">Represent the Type of specific activity</param>
        /// <param name="cancellationToken">Cancel the operation if triggered</param>
        /// <returns>Return a <see cref="Task"/> that is contain the count of one last hour activities for this IP</returns>
        Task<int> CountLastHourOfIpAsync(string ip, ActivityType type, CancellationToken cancellationToken = default);

    }
}
