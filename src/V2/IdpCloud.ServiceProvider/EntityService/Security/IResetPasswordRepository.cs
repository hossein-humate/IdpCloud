using IdpCloud.DataProvider.Entity.Security;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Security
{
    /// <summary>
    /// Service to provide Data Logics for ResetPassword Entity
    /// </summary>
    public interface IResetPasswordRepository
    {
        /// <summary>
        /// Add new <see cref="ResetPassword"/> record to the database
        /// </summary>
        /// <param name="resetPassword">Represent the record parameters data</param>
        /// <param name="cancellationToken">Cancel the operation if triggered</param>
        /// <returns>Return a Task <see cref="Task"/> that is contain the exact <see cref="ResetPassword"/> record currently saved in database</returns>
        Task<ResetPassword> AddAsync(ResetPassword resetPassword, CancellationToken cancellationToken = default);

        /// <summary>
        /// Find the Related record of ResetPassword entity by provided UserId and Secret values 
        /// that is active right now and ready to use for change password operation
        /// </summary>
        /// <param name="userId">specific UserId used to search the resetpassword record</param>
        /// <param name="secret">Represent the random secure string value and user to find the resetPassword record</param>
        /// <param name="cancellationToken">Cancel the operation if triggered</param>
        /// <returns>Return a Task <see cref="Task"/> that is contain the exact <see cref="ResetPassword"/> record with realted entity</returns>
        Task<ResetPassword> FindByUserIdAndSecretAsync(Guid userId, string secret, CancellationToken cancellationToken = default);

        /// <summary>
        /// Edit Parameters value of specific <see cref="ResetPassword"/> record in the database
        /// </summary>
        /// <param name="resetPassword">Represent the record parameters data</param>
        /// <returns>Return <see cref="ResetPassword"/> record currently updated in database</returns>
        ResetPassword Update(ResetPassword resetPassword);
    }
}
