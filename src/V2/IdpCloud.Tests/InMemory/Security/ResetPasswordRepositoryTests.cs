using IdpCloud.Common;
using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.Security;
using IdpCloud.ServiceProvider.EntityService.Security;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.InMemory.Identity.Security
{
    /// <summary>
    /// ResetPassword repository Test Suit
    /// </summary>
    public class ResetPasswordRepositoryTests : TestBase
    {
        private readonly IResetPasswordRepository _resetPasswordRepository;
        private readonly EfCoreContext _efDbContext;

        /// <summary>
        /// Initialises an instance of <see cref="ResetPasswordRepositoryTests"/>.
        /// </summary>
        public ResetPasswordRepositoryTests()
        {
            _efDbContext = new EfCoreContext(GetMockContextOptions());
            _efDbContext.Database.EnsureDeleted();
            _efDbContext.Database.EnsureCreated();
            _resetPasswordRepository = new ResetPasswordRepository(_efDbContext);
        }

        /// <summary>
        /// Asserts <see cref="ResetPasswordRepository.AddAsync(ResetPassword, System.Threading.CancellationToken)"/>
        /// adds the ResetPassword</summary> when given <see cref="ResetPassword"/>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task AddAsync_ShouldAddAndReturnResetPassword_WhenGivenResetPassword()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var user = new User() { UserId = userId };
            var resetPassword = new ResetPassword()
            {
                User = user,
                Expiry = DateTime.UtcNow.AddHours(1),
                Secret = Cryptography.GenerateSecret(32)
            };

            //Act
            var resetPasswordResult = await _resetPasswordRepository.AddAsync(resetPassword, default);

            //Assert
            Assert.StrictEqual(resetPasswordResult, resetPassword);
        }

        /// <summary>
        /// Asserts <see cref="ResetPasswordRepository.FindByUserIdAndSecretAsync(Guid, string, System.Threading.CancellationToken)"/>
        /// returns the ResetPassword</summary> for active user when given valid <see cref="User.UserId"/> and <see cref="ResetPassword.Secret"/>
        ///for active ResetPassword status
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task FindByUserIdAndSecretAsync_ShouldReturnResetPassword_ForGivenActiveResetPasswordAndValidUserIdAndSecret()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var user = new User() { UserId = userId };
            var secret = Cryptography.GenerateSecret(32);
            var resetPassword = new ResetPassword()
            {
                User = user,
                Expiry = DateTime.UtcNow.AddHours(1),
                Secret = secret
            };

            //Act
            await _resetPasswordRepository.AddAsync(resetPassword, default);
            await _efDbContext.SaveChangesAsync();
            var resetPasswordResult = await _resetPasswordRepository.FindByUserIdAndSecretAsync(userId, secret, default);

            //Assert
            Assert.NotNull(resetPasswordResult);
            Assert.StrictEqual(resetPasswordResult, resetPassword);
            Assert.StrictEqual(resetPasswordResult.User, user);
        }

        /// <summary>
        /// Asserts <see cref="ResetPasswordRepository.FindByUserIdAndSecretAsync(Guid, string, System.Threading.CancellationToken)"/>
        /// returns the ResetPassword</summary> for active user when given valid <see cref="User.UserId"/> and <see cref="ResetPassword.Secret"/>
        ///for Inactive ResetPassword status
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task FindByUserIdAndSecretAsync_ShouldNotReturnResetPassword_ForInactiveResetPasswordAndValidUserIdAndSecret()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var user = new User() { UserId = userId };
            var secret = Cryptography.GenerateSecret(32);
            var resetPassword = new ResetPassword()
            {
                User = user,
                Expiry = DateTime.UtcNow.AddHours(1),
                Secret = secret,
                Active = false
            };

            //Act
            await _resetPasswordRepository.AddAsync(resetPassword, default);
            await _efDbContext.SaveChangesAsync();
            var resetPasswordResult = await _resetPasswordRepository.FindByUserIdAndSecretAsync(userId, secret, default);

            //Assert
            Assert.Null(resetPasswordResult);
        }

        /// <summary>
        /// Asserts <see cref="ResetPasswordRepository.Update(ResetPassword)"/>
        /// updates the <see cref="ResetPassword"/> for given ResetPassword.
        /// returns the updated ResetPassword
        /// </summary> 
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Update_ShouldReturnUpdatedResetPassword_WhenGivenResetPassword()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var user = new User() { UserId = userId };
            var secret = Cryptography.GenerateSecret(32);
            var resetPassword = new ResetPassword()
            {
                User = user,
                Expiry = DateTime.UtcNow.AddHours(1),
                Secret = secret
            };

            //Act
            await _resetPasswordRepository.AddAsync(resetPassword, default);
            await _efDbContext.SaveChangesAsync();

            resetPassword.Active = false;

            var resetPasswordUpdateResult = _resetPasswordRepository.Update(resetPassword);
            await _efDbContext.SaveChangesAsync();

            //Assert
            Assert.NotNull(resetPasswordUpdateResult);
            Assert.False(resetPasswordUpdateResult.Active);
        }

        internal void Dispose()
        {
            _efDbContext.Dispose();
        }
    }
}
