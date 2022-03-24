using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.ServiceProvider.EntityService.SSO;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Repositories
{
    /// <summary>
    /// Test suite for <see cref="NewUserSessionRepository"/>.
    /// </summary>
    public class UserSessionReposiotryTests
    {
        private readonly UserSessionRepository _newUserSessionRepository;
        private readonly Mock<EfCoreContext> _mockDbContext;
        private readonly Mock<DbSet<UserSession>> _mockUserSessionDbSet;
        private readonly DbContextHelper _dbContextHelper = new();
        private readonly Guid _userSessionId = Guid.NewGuid();
        private readonly Guid _softwareId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _personId = Guid.NewGuid();

        /// <summary>
        /// Initialises an instance of <see cref="LoginControllerTests"/>.
        /// </summary>
        public UserSessionReposiotryTests()
        {
            List<UserSession> _userSession = GetUserSessions();
            var options = new DbContextOptionsBuilder<EfCoreContext>().UseSqlServer().Options;
            _mockDbContext = new Mock<EfCoreContext>(options);
            _mockUserSessionDbSet = _dbContextHelper.CreateDbSetMock(_userSession.AsQueryable());
            _mockDbContext.Setup(x => x.UserSessions).Returns(_mockUserSessionDbSet.Object);
            _newUserSessionRepository = new UserSessionRepository(_mockDbContext.Object);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "NewUserSessionRepository.GetUserSessionByIdAndStatus(Guid?, Status, CancellationToken)" />
        /// returns the Usersession with user and Software
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetUserSessionByIdAndStatus_ShouldReturnUserSessionWithUserAndSoftware_ForValidUserSession()
        {
            //Act
            var resultUserSession = await _newUserSessionRepository.GetUserSessionByIdAndStatus(_userSessionId, Status.Active);

            //Assert
            Assert.NotNull(resultUserSession);
            Assert.Equal(resultUserSession.UserSessionId, _userSessionId);
            Assert.Equal(resultUserSession.Software.SoftwareId, _softwareId);
            Assert.Equal(resultUserSession.User.UserId, _userId);
            Assert.NotNull(resultUserSession.User.Language);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "NewUserSessionRepository.GetUserSessionByIdAndStatus(Guid?, Status, CancellationToken)" />
        /// returns null when UserSession is inValid and status is Active.
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetUserSessionByIdAndStatus_ShouldReturnNull_ForInvalidUserSessionId()
        {
            //Act
            var resultUserSession = await _newUserSessionRepository.GetUserSessionByIdAndStatus(Guid.NewGuid(), Status.Active);

            //Assert
            Assert.Null(resultUserSession);
        }

        /// <summary>
        /// Asserts that
        /// <see cref = "NewUserSessionRepository.GetUserSessionByIdAndStatus(Guid?, Status, CancellationToken)" />
        /// returns null when UserSession status is Deactive.
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetUserSessionByIdAndStatus_ShouldReturnNull_ForInvalidUserSessionStatus()
        {
            //Act
            var resultUserSession = await _newUserSessionRepository.GetUserSessionByIdAndStatus(Guid.NewGuid(), Status.DeActive);

            //Assert
            Assert.Null(resultUserSession);
        }

        /// <summary>
        /// Deactivates the Usersession when UserSession is valid
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an void unit test.</returns>
        [Fact]
        public void DeactivateUserSession_ShouldDeactivateUserSession_WhenUserSessionIsValid()
        {
            //Arrange
            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession { UserSessionId = userSessionId };

            _mockUserSessionDbSet
                .Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userSession);

            //Act
            _newUserSessionRepository.DeactivateUserSession(userSessionId);

            //Assert
            _mockUserSessionDbSet.Verify(m => m.FindAsync(new object[] { userSessionId }, default), Times.Once());
        }
        /// <summary>
        /// Deactivates the Usersession when UserSession is not valid/not found
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an void unit test.</returns>
        [Fact]
        public void DeactivateUserSession_ShouldDoNothing_WhenUserSessionIsNotValid()
        {
            //Arrange
            var userSessionId = Guid.NewGuid();

            _mockUserSessionDbSet
                .Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as UserSession);

            //Act
            _newUserSessionRepository.DeactivateUserSession(userSessionId);

            //Assert
            _mockUserSessionDbSet.Verify(m => m.FindAsync(new object[] { userSessionId }, default), Times.Once());
            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Never());
        }

        private List<UserSession> GetUserSessions()
        {
            var software = new Software() { SoftwareId = _softwareId };

            var user = new User()
            {
                UserId = _userId,
                Firstname = "TestFirstName",
                Language = new DataProvider.Entity.BaseInfo.Language { LanguageId = 1 }
            };

            var userSessionOne = new UserSession()
            {
                UserSessionId = _userSessionId,
                Status = Status.Active,
                Software = software,
                User = user
            };
            var userSessionTwo = new UserSession()
            {
                UserSessionId = Guid.NewGuid(),
                Status = Status.Active
            };
            var userSessionThree = new UserSession()
            {
                UserSessionId = Guid.NewGuid(),
                Status = Status.DeActive
            };

            var userSessions = new List<UserSession>() { userSessionOne, userSessionTwo, userSessionThree };

            return userSessions;
        }
    }
}
