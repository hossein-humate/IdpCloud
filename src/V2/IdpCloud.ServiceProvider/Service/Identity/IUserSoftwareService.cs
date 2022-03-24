using IdpCloud.DataProvider.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service.Identity
{
    /// <summary>
    /// Service to provide business logic for UserSoftware Entity and UserSoftware Controller
    /// </summary>
    public interface IUserSoftwareService
    {
        /// <summary>
        /// Service method that would get software list for the current authenticated User the 
        /// </summary>
        /// <param name="userId">Represent authenticated userId</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation that is contain <see cref="IEnumerable{Software}"/></returns>
        Task<IEnumerable<Software>> SoftwareList(Guid userId, CancellationToken cancellationToken = default);
    }
}
