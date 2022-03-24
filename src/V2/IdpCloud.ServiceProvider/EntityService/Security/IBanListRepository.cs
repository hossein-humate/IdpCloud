using IdpCloud.DataProvider.Entity.Security;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Security
{
    /// <summary>
    /// Service to provide data logics for BanList Entity
    /// </summary>
    public interface IBanListRepository
    {
        /// <summary>
        /// Add new <see cref="BanList"/> record to the database
        /// </summary>
        /// <param name="banList">Represent the record parameters data</param>
        /// <param name="cancellationToken">Cancel the operation if triggered</param>
        /// <returns>Return a Task <see cref="Task"/> that is contain the exact <see cref="BanList"/> record currently saved in database</returns>
        Task<BanList> AddAsync(BanList banList, CancellationToken cancellationToken = default);

        /// <summary>
        /// Search into the BanList table for a specific IP Address, If the input ip address exist in
        /// BanList table and the ReleaseDate(type of <see cref="DateTime"/>) value is not reached
        /// to the scheduled time yet, it will return the exact record values
        /// </summary>
        /// <param name="ip">specific Ip address used to search into the banlist table</param>
        /// <param name="cancellationToken">Cancel the operation if triggered</param>
        /// <returns>Return a Task <see cref="Task"/> that is contain a <see cref="BanList"/> type.</returns>
        Task<BanList> GetBannedIPAsync(string ip, CancellationToken cancellationToken = default);

        /// <summary>
        /// Edit Parameters value of specific <see cref="BanList"/> record in the database
        /// </summary>
        /// <param name="banList">Represent the record parameters data</param>
        /// <returns>Return <see cref="BanList"/> record currently updated in database</returns>
        BanList Update(BanList banList);
    }
}
