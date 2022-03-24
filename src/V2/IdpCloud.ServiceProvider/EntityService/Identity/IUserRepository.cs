using IdpCloud.DataProvider.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Identity
{
    /// <summary>
    /// The User repository
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get user by UserName or Email and Password
        /// </summary>
        /// <param name="usernameOrEmail">The <see cref="User.Username/ User.Email"/>UsernameOrEmail</param>
        /// <param name="password">The <see cref="User.PasswordHash /User.PasswordSalt"/>Password</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>The<see cref="User"/>User entity</returns>
        Task<User> GetUserByUserNameOrEmailAndPassword(string usernameOrEmail, string password,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Find the User by UserId
        /// </summary>
        /// <param name="userId">The <see cref="User.UserId"/>UserId</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns></returns>
        Task<User> FindById(Guid? userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>The<see cref="User"/>List of Users</returns>
        Task<IEnumerable<User>> GetAll(CancellationToken cancellationToken = default);

        /// <summary>
        /// Update User object
        /// </summary>
        /// <param name="user">User</param>
        User Update(User user);

        /// <summary>
        /// Virtual Delete it will set the DeleteDate of the provided user record, this is not a physical delete
        /// </summary>
        /// <param name="user">Represent the User Record that should be delete</param>
        void Delete(User user);

        /// <summary>
        /// Add new <see cref="User"/> record to the database
        /// </summary>
        /// <param name="user">Represent the record parameters data</param>
        /// <returns>Return a <see cref="Task"/>that is contain the exact <see cref="User"/> record currently saved in database</returns>
        Task<User> Add(User user);

        /// <summary>
        /// Get user by Email
        /// </summary>
        /// <param name="email">The <see cref="User.Email"/>User Email</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>Return a <see cref="Task"/> that contain the User record with the same given email</returns>
        Task<User> GetByEmail(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get User by username
        /// </summary>
        /// <param name="username">The <see cref="User.Username"/> Username prameter</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>Return the User record if there is any that has same username value with provided input string</returns>
        Task<User> GetByUsername(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">The userId</param>
        /// <param name="emailConfirmationSecret"> The email confirmation secret</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>Return a <see cref=Task<User>/> that contain the User record with the same given emailConfirmationSecret and UserId</returns>
        Task<User> GetByEmailConfirmationSecretAndId(Guid? userId, string emailConfirmationSecret, CancellationToken cancellationToken = default);
    }
}
