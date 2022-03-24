using IdpCloud.DataProvider.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Identity
{
    /// <summary>
    /// User Software repository provide all Queries related to the UserSoftware entity in the database
    /// </summary>
    public interface IUserSoftwareRepository
    {
        /// <summary>
        /// Query to find list of software those are already access to the given userId
        /// </summary>
        /// <param name="userId">Represent the authenticated userId</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>Return a <see cref="Task"/> that contain the List of software</returns>
        Task<IEnumerable<Software>> GetSoftwareListByUserId(Guid userId, CancellationToken cancellationToken = default);
    }
}
