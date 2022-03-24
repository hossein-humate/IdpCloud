using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.ServiceProvider.EntityService.SSO;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Services
{
    /// <summary>
    /// Test suite for <see cref="CurrentUserSessionService"/>.
    /// </summary>
    public class CurrentUserSessionServiceTests
    {
        private readonly ICurrentUserSessionService _currentUserSessionService;
        private readonly Mock<IUserSessionRepository> _mockUserSessionRepository = new();
        private readonly JwtSecurityTokenHandler _jwtSettingTokenHandler = new();
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
        private static Guid roleId = Guid.NewGuid();

        private readonly Dictionary<string, string> inMemorySettings = new()
        {
            { "Application:AdministratorRoleId", roleId.ToString() }
        };
        public CurrentUserSessionServiceTests()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                                .AddInMemoryCollection(inMemorySettings)
                                .Build();

            _currentUserSessionService = new CurrentUserSessionService(
                _mockUserSessionRepository.Object,
                _mockHttpContextAccessor.Object,
                _jwtSettingTokenHandler,
                configuration
               );
        }

        /// <summary>
        /// Asserts than when calling <see cref="CurrentUserSessionService.UserSession"/>
        /// will populate the UserSession with Software but should not call CurrentSoftwareService 
        /// when security token header is not null
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task UserSession_ShouldPopulateAndReturnUserSesion_WhenUserSessionIsNull()
        {
            //Arrange
            var mockHeaders = new Mock<IHeaderDictionary>();

            var softwareId = Guid.NewGuid();
            var software = new Software() { SoftwareId = softwareId };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Software = software };


            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA=="));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            IDictionary<string, object> claims = new Dictionary<string, object>
            {
                { "UserSessionId", userSessionId }
            };
            var SecurityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "test.com",
                Audience = "testaudience",
                Expires = DateTime.UtcNow.AddMinutes(5),
                Claims = claims,
                SigningCredentials = credentials
            };

            var jwtSecurityToken = handler.CreateJwtSecurityToken(SecurityTokenDescriptor);
            var token = handler.WriteToken(jwtSecurityToken);

            mockHeaders
                    .Setup(m => m.ContainsKey("X-Security-Token"))
                    .Returns(true);

            mockHeaders
                .Setup(m => m["X-Security-Token"])
                .Returns(token);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            _mockUserSessionRepository
                .Setup(us => us.GetUserSessionByIdAndStatus(It.IsAny<Guid>(),
                It.IsAny<Status>(),
                 It.IsAny<CancellationToken>()))
                .ReturnsAsync(userSession);

            //Act
            var userSessionResult = _currentUserSessionService.UserSession;

            //Assert
            Assert.NotNull(userSessionResult);
            Assert.Equal(userSessionResult.UserSessionId, userSessionId);

            Assert.NotNull(userSessionResult.Software);
            Assert.Equal(userSessionResult.Software.SoftwareId, softwareId);

            _mockUserSessionRepository.Verify(u => u.GetUserSessionByIdAndStatus(userSessionId, Status.Active, default), Times.Once());
            _mockHttpContextAccessor.Verify(u => u.HttpContext, Times.Once());
            mockHeaders.Verify(u => u.ContainsKey("X-Security-Token"), Times.Once());
        }

        /// <summary>
        /// Asserts than when calling <see cref="CurrentUserSessionService.UserSession"/>
        /// will populate null/empty UserSession 
        /// when security token header is null
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task UserSession_ShouldNotPopulateUserSession_WhenUserSessionAndTokenHeaderIsNull()
        {
            //Arrange

            var softwareId = Guid.NewGuid();
            var software = new Software() { SoftwareId = softwareId };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId };

            _mockHttpContextAccessor
               .SetupGet(m => m.HttpContext)
               .Returns(null as HttpContext);

            //Act
            var userSessionResult = _currentUserSessionService.UserSession;

            //Assert
            Assert.Null(userSessionResult);

            _mockUserSessionRepository.Verify(u => u.GetUserSessionByIdAndStatus(userSessionId, Status.Active, default), Times.Never());
            _mockHttpContextAccessor.Verify(u => u.HttpContext, Times.Once());
        }

        /// <summary>
        /// Asserts than when calling <see cref="CurrentUserSessionService.UserSession"/>
        /// will populate the null UserSession 
        /// when security token null
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task UserSession_ShouldPopulateNullUserSession_WhenUserSessionAndTokenIsNull()
        {
            //Arrange
            var mockHeaders = new Mock<IHeaderDictionary>();

            var softwareId = Guid.NewGuid();
            var software = new Software() { SoftwareId = softwareId };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId };

            mockHeaders
                    .Setup(m => m.ContainsKey("X-Security-Token"))
                    .Returns(true);

            mockHeaders
                .Setup(m => m["X-Security-Token"])
                .Returns(null as string);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            //Act
            var userSessionResult = _currentUserSessionService.UserSession;

            //Assert
            Assert.Null(userSessionResult);

            _mockUserSessionRepository.Verify(u => u.GetUserSessionByIdAndStatus(userSessionId, Status.Active, default), Times.Never());
            _mockHttpContextAccessor.Verify(u => u.HttpContext, Times.Once());
            mockHeaders.Verify(u => u.ContainsKey("X-Security-Token"), Times.Once());
        }


        /// <summary>
        /// Asserts than when calling <see cref="CurrentUserSessionService.UserSession"/>
        /// will populate null usersession
        /// when security token has missing user sesion id
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task UserSession_ShouldPopulateNullUserSession_WhenUserSessionIdIsNullInToken()
        {
            //Arrange
            var mockHeaders = new Mock<IHeaderDictionary>();

            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA=="));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var SecurityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "test.com",
                Audience = "testaudience",
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = credentials
            };

            var jwtSecurityToken = handler.CreateJwtSecurityToken(SecurityTokenDescriptor);
            var token = handler.WriteToken(jwtSecurityToken);

            mockHeaders
                    .Setup(m => m.ContainsKey("X-Security-Token"))
                    .Returns(true);

            mockHeaders
                .Setup(m => m["X-Security-Token"])
                .Returns(token);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            //Act
            var userSessionResult = _currentUserSessionService.UserSession;

            //Assert
            Assert.Null(userSessionResult);

            _mockUserSessionRepository.Verify(u => u.GetUserSessionByIdAndStatus(It.IsAny<Guid>(), It.IsAny<Status>(), default), Times.Never());
            _mockHttpContextAccessor.Verify(u => u.HttpContext, Times.Once());
            mockHeaders.Verify(u => u.ContainsKey("X-Security-Token"), Times.Once());
        }

        /// <summary>
        /// Asserts than when calling <see cref="CurrentUserSessionService.UserSession"/>
        /// willreturn user session 
        /// when user session is not null
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task UserSession_ShouldReturnUserSesion_WhenUserSessionIsNotNull()
        {
            //Arrange
            var mockHeaders = new Mock<IHeaderDictionary>();

            var softwareId = Guid.NewGuid();
            var software = new Software() { SoftwareId = softwareId };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Software = software };


            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA=="));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            IDictionary<string, object> claims = new Dictionary<string, object>
            {
                { "UserSessionId", userSessionId }
            };
            var SecurityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "test.com",
                Audience = "testaudience",
                Expires = DateTime.UtcNow.AddMinutes(5),
                Claims = claims,
                SigningCredentials = credentials
            };

            var jwtSecurityToken = handler.CreateJwtSecurityToken(SecurityTokenDescriptor);
            var token = handler.WriteToken(jwtSecurityToken);

            mockHeaders
                    .Setup(m => m.ContainsKey("X-Security-Token"))
                    .Returns(true);

            mockHeaders
                .Setup(m => m["X-Security-Token"])
                .Returns(token);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            _mockUserSessionRepository
                .Setup(us => us.GetUserSessionByIdAndStatus(It.IsAny<Guid>(),
                It.IsAny<Status>(),
                 It.IsAny<CancellationToken>()))
                .ReturnsAsync(userSession);

            //Act
            var userSessionResult = _currentUserSessionService.UserSession;

            //Assert
            Assert.NotNull(userSessionResult);
            Assert.Equal(userSessionResult.UserSessionId, userSessionId);

            Assert.NotNull(userSessionResult.Software);
            Assert.Equal(userSessionResult.Software.SoftwareId, softwareId);

            _mockUserSessionRepository.Verify(u => u.GetUserSessionByIdAndStatus(userSessionId, Status.Active, default), Times.Once());
            _mockHttpContextAccessor.Verify(u => u.HttpContext, Times.Once());
            mockHeaders.Verify(u => u.ContainsKey("X-Security-Token"), Times.Once());

            _mockUserSessionRepository.Reset();
            _mockHttpContextAccessor.Reset();

            var userSessionResultTwo = _currentUserSessionService.UserSession;

            Assert.NotNull(userSessionResult);
            Assert.Equal(userSessionResult.UserSessionId, userSessionId);

            Assert.NotNull(userSessionResult.Software);
            Assert.Equal(userSessionResult.Software.SoftwareId, softwareId);

            _mockHttpContextAccessor.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Asserts than when calling <see cref="CurrentUserSessionService.IsAdministrator"/>
        /// will return IsAdministartor true and roleId is same to Configuration
        /// when user session is not null
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task IsAdministrator_ShouldReturnIsAdministratorTrue()
        {
            //Arrange
            var mockHeaders = new Mock<IHeaderDictionary>();

            var softwareId = Guid.NewGuid();
            var software = new Software() { SoftwareId = softwareId };

            var userRoleId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var userRole = new UserRole() { UserRoleId = userRoleId, UserId = userId, RoleId = roleId };
            var userRoles = new List<UserRole>() { userRole };

            var user = new User() { UserId = userId, UserRoles = userRoles };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Software = software, User = user };

            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA=="));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            IDictionary<string, object> claims = new Dictionary<string, object>
            {
                { "UserSessionId", userSessionId }
            };
            var SecurityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "test.com",
                Audience = "testaudience",
                Expires = DateTime.UtcNow.AddMinutes(5),
                Claims = claims,
                SigningCredentials = credentials
            };

            var jwtSecurityToken = handler.CreateJwtSecurityToken(SecurityTokenDescriptor);
            var token = handler.WriteToken(jwtSecurityToken);

            mockHeaders
                    .Setup(m => m.ContainsKey("X-Security-Token"))
                    .Returns(true);

            mockHeaders
                .Setup(m => m["X-Security-Token"])
                .Returns(token);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            _mockUserSessionRepository
                .Setup(us => us.GetUserSessionByIdAndStatus(It.IsAny<Guid>(),
                It.IsAny<Status>(),
                 It.IsAny<CancellationToken>()))
                .ReturnsAsync(userSession);


            //Act
            var usersession = _currentUserSessionService.UserSession;
            var isAdmin = _currentUserSessionService.IsAdministrator;

            //Assert
            Assert.NotNull(usersession);
            Assert.Equal(usersession.UserSessionId, userSessionId);
            Assert.True(isAdmin);

            _mockUserSessionRepository.Verify(u => u.GetUserSessionByIdAndStatus(userSessionId, Status.Active, default), Times.Once());
            _mockHttpContextAccessor.Verify(u => u.HttpContext, Times.Once());
            mockHeaders.Verify(u => u.ContainsKey("X-Security-Token"), Times.Once());

        }

        /// <summary>
        /// Asserts than when calling <see cref="CurrentUserSessionService.IsAdministrator"/>
        /// will return IsAdministartor false
        /// when user session is not null and roleId is not same to Configuration
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task IsAdministrator_ShouldReturnIsAdministratorFalse()
        {
            //Arrange
            var mockHeaders = new Mock<IHeaderDictionary>();

            var softwareId = Guid.NewGuid();
            var software = new Software() { SoftwareId = softwareId };

            var userRoleId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var userRole = new UserRole() { UserRoleId = userRoleId, UserId = userId, RoleId = Guid.NewGuid() };
            var userRoles = new List<UserRole>() { userRole };

            var user = new User() { UserId = userId, UserRoles = userRoles };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Software = software, User = user };

            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA=="));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            IDictionary<string, object> claims = new Dictionary<string, object>
            {
                { "UserSessionId", userSessionId }
            };
            var SecurityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "test.com",
                Audience = "testaudience",
                Expires = DateTime.UtcNow.AddMinutes(5),
                Claims = claims,
                SigningCredentials = credentials
            };

            var jwtSecurityToken = handler.CreateJwtSecurityToken(SecurityTokenDescriptor);
            var token = handler.WriteToken(jwtSecurityToken);

            mockHeaders
                    .Setup(m => m.ContainsKey("X-Security-Token"))
                    .Returns(true);

            mockHeaders
                .Setup(m => m["X-Security-Token"])
                .Returns(token);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            _mockUserSessionRepository
                .Setup(us => us.GetUserSessionByIdAndStatus(It.IsAny<Guid>(),
                It.IsAny<Status>(),
                 It.IsAny<CancellationToken>()))
                .ReturnsAsync(userSession);


            //Act
            var usersession = _currentUserSessionService.UserSession;
            var isAdmin = _currentUserSessionService.IsAdministrator;

            //Assert
            Assert.NotNull(usersession);
            Assert.Equal(usersession.UserSessionId, userSessionId);
            Assert.False(isAdmin);

            _mockUserSessionRepository.Verify(u => u.GetUserSessionByIdAndStatus(userSessionId, Status.Active, default), Times.Once());
            _mockHttpContextAccessor.Verify(u => u.HttpContext, Times.Once());
            mockHeaders.Verify(u => u.ContainsKey("X-Security-Token"), Times.Once());

        }
    }
}
