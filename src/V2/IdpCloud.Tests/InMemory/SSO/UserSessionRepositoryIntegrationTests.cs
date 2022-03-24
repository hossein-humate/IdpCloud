using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.ServiceProvider.EntityService.SSO;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.InMemory
{
    public class UserSessionRepositoryIntegrationTests : TestBase
    {
        private readonly IUserSessionRepository _newUserSessionRepository;
        private readonly EfCoreContext _efDbContext;
        private readonly Guid _userId = Guid.NewGuid();
        private readonly int _OrganisationId = 1;
        private readonly Guid _softwareId = Guid.NewGuid();
        public UserSessionRepositoryIntegrationTests()
        {
            _efDbContext = new EfCoreContext(GetMockContextOptions());
            _efDbContext.Database.EnsureDeleted();
            _efDbContext.Database.EnsureCreated();
            _newUserSessionRepository = new UserSessionRepository(_efDbContext);
        }

        /// <summary>
        /// Asserts <see cref="NewUserSessionRepository.GetUserSessionByIdAndStatus(Guid?, Status, CancellationToken)"/>
        /// returns UserSession</summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetUserSessionByIdAndStatus_ShouldReturnUserSession()
        {
            //Arrange
            var status = Status.Active;
            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession()
            {
                UserSessionId = userSessionId,
                Status = status,
                User = new User()
                {
                    UserId = _userId,
                    Organisation = new Organisation
                    {
                        OrganisationId = _OrganisationId,
                    }
                },
                Software = new Software() { SoftwareId = _softwareId }
            };

            //Act
            await _newUserSessionRepository.Add(userSession);
            await _efDbContext.SaveChangesAsync();
            var userSessionResult = await _newUserSessionRepository.GetUserSessionByIdAndStatus(userSessionId, status);

            //Assert
            Assert.NotNull(userSessionResult);
            Assert.Equal(userSessionResult.UserSessionId, userSessionId);
            Assert.Equal(userSessionResult.Status, status);
            Assert.NotNull(userSessionResult.User.Organisation);
            Assert.Equal(userSessionResult.User.Organisation.OrganisationId, _OrganisationId);
            Assert.Equal(userSessionResult.UserId, _userId);
            Assert.Equal(userSessionResult.SoftwareId, _softwareId);
            Assert.Equal(userSessionResult.User.UserId, _userId);
            Assert.Equal(userSessionResult.Software.SoftwareId, _softwareId);

        }

        /// <summary>
        /// Asserts <see cref="NewUserSessionRepository.DeactivateUserSession(Guid?, CancellationToken)"/>
        /// returns nothing and Update UserSession status as Deactive</summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task DeactivateUserSession_ShouldUpdateUserSessionStatus_ForValidUserSession()
        {
            //Arrange
            var status = Status.Active;
            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession()
            {
                UserSessionId = userSessionId,
                Status = status,
                User = new User() { UserId = _userId },
                Software = new Software() { SoftwareId = _softwareId }
            };

            //Act
            await _newUserSessionRepository.Add(userSession);
            _newUserSessionRepository.DeactivateUserSession(userSessionId, default);

            await _efDbContext.SaveChangesAsync();

            var userSessionResult = await _newUserSessionRepository.GetUserSessionByIdAndStatus(userSessionId, Status.DeActive);

            //Assert
            Assert.NotNull(userSessionResult);
        }

        /// <summary>
        /// Asserts <see cref="NewUserSessionRepository.DeactivateUserSession(Guid?, CancellationToken)"/>
        /// returns nothing and do not Update UserSession status as Deactive for Invalid UserSession</summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task DeactivateUserSession_ShouldNotUpdateUserSessionStatus_ForInValidUserSession()
        {
            //Arrange
            var status = Status.Active;
            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession()
            {
                UserSessionId = userSessionId,
                Status = status,
                User = new User() { UserId = _userId },
                Software = new Software() { SoftwareId = _softwareId }
            };

            //Act
            await _newUserSessionRepository.Add(userSession);
            await _efDbContext.SaveChangesAsync();
            _newUserSessionRepository.DeactivateUserSession(Guid.NewGuid(), default);
            var userSessionResult = await _newUserSessionRepository.GetUserSessionByIdAndStatus(userSessionId, Status.DeActive);

            //Assert
            Assert.Null(userSessionResult);

        }

        /// <summary>
        /// Asserts <see cref="NewUserSessionRepository.Add(UserSession, CancellationToken)(Guid?, CancellationToken)"/>
        /// adds the UserSession</summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Add_ShouldAddUserSession()
        {
            //Arrange
            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Status = Status.Active };

            //Act
            await _newUserSessionRepository.Add(userSession);
            await _efDbContext.SaveChangesAsync();
            var userSessionResult = await _newUserSessionRepository.FindById(userSessionId, default);

            //Assert
            Assert.StrictEqual(userSession, userSessionResult);

        }

        /// <summary>
        /// Asserts <see cref="NewUserSessionRepository.Update(UserSession, CancellationToken)"/>
        /// Update the UserSession</summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Update_ShouldUpdateUserSession_WhenGivenUserSsession()
        {
            //Arrange
            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Status = Status.Active };

            //Act
            await _newUserSessionRepository.Add(userSession);
            await _efDbContext.SaveChangesAsync();

            userSession.Status = Status.DeActive;
            _newUserSessionRepository.Update(userSession);

            var userSessionResult = await _newUserSessionRepository.FindById(userSessionId, default);

            //Assert
            Assert.Equal(Status.DeActive, userSessionResult.Status);

        }
        internal void Dispose()
        {
            _efDbContext.Dispose();
        }
    }
}
