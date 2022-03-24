using IdpCloud.DataProvider.Entity.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Identity
{
    /// <summary>
    /// Role repository provide all Queries related to the Role and UserRole entities in the database
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Add new <see cref="UserRole"/> record to the database
        /// </summary>
        /// <param name="user">Represent the record parameters data</param>
        /// <returns>Return a <see cref="Task"/>that is contain the exact <see cref="UserRole"/> record currently saved in database</returns>
        Task<UserRole> AddUserRole(UserRole userRole);

        /// <summary>
        /// Virtual Delete it will set the DeleteDate of the provided userRole record, this is not a physical delete
        /// </summary>
        /// <param name="user">Represent the User Record that should be delete</param>
        void DeleteUserRole(UserRole userRole);

        /// <summary>
        /// Find the record of UserRole entity by provided UserId and RoleId
        /// </summary>
        /// <param name="userId">Specific UserId used to find userRole</param>
        /// <param name = "cancellationToken"> A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns></returns>
        Task<UserRole> FindByUserId(Guid userId, CancellationToken cancellationToken = default);

    }
}
