
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.SSO.Request.UserSession;
using IdpCloud.Sdk.Model.SSO.Response.UserSession;
using IdpCloud.ServiceProvider.EntityService.SSO;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Services
{
    /// <summary>
    /// Test suite for <see cref="RefreshTokenServiceTests"/>.
    /// </summary>
    public class RefreshTokenServiceTests
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly Mock<ICurrentUserSessionService> _mockCurrentUserSessionService = new();
        private readonly Mock<IUserSessionRepository> _mockUserSessionRepository = new();
        private readonly Mock<IJwtAuthenticationService> _mockJwtAuthenticationService = new();
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
        /// <summary>
        /// Initialises an instance of <see cref="RefreshTokenServiceTests"/> 
        /// </summary>
        public RefreshTokenServiceTests()
        {
            _refreshTokenService = new RefreshTokenService(
                     _mockCurrentUserSessionService.Object,
                     _mockUserSessionRepository.Object,
                     _mockJwtAuthenticationService.Object,
                     _mockHttpContextAccessor.Object);
        }

        /// <summary>
        /// Asserts that <see cref="RefreshTokenService.CreateRefreshToken(string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="RefreshTokenResponse"/>containig <see cref="RequestResult"/> RefreshTokenIsDisable 
        /// and not JWT and refreshToken is created
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task CreateRefreshToken_ShouldNotCreateRefreshToken_ReturnsresultCodeRefreshTokenIsDisable_WhenJWTHasRefreshIsfalse()
        {
            //Arrange

            var clientIp = "TestClientIp";
            var userAgent = "TestUserAgent";

            var jwtSetting = new JwtSetting() { JwtSettingId = 1, HasRefresh = false, RefreshExpireMinute = 5, Secret = "iGfCtXUgK3ulAKvsNu8szl9mUeaYTRhj2EEHCfuTilZ4RKPBRh76YddLUFMxyPRHnZ3XOBvCSMLrokcS6kzpe9pdhonuIdMbXK9X" };

            var software = new Software() { SoftwareId = Guid.NewGuid(), JwtSetting = jwtSetting };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, RefreshToken = "TestRefreshToken", Software = software };

            var cancellationToken = CancellationToken.None;

            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "TestRefreshToken",
                ClientIp = clientIp,
                ClientUserAgent = userAgent
            };

            _mockCurrentUserSessionService
                     .Setup(us => us.UserSession)
                     .Returns(userSession);

            //Act
            var result = await _refreshTokenService.CreateRefreshToken(refreshTokenRequest, cancellationToken);

            //Arrange
            Assert.NotNull(result);
            Assert.Equal(Sdk.Model.RequestResult.RefreshTokenIsDisable, result.ResultCode);
            _mockCurrentUserSessionService.Verify(us => us.UserSession, Times.Once());
            _mockUserSessionRepository.Verify(us => us.Add(It.IsAny<UserSession>(), cancellationToken), Times.Never);
            _mockJwtAuthenticationService.Verify(us => us.CreateTokenAuthenticationAsync(It.IsAny<UserSession>(), cancellationToken), Times.Never);
            _mockJwtAuthenticationService.Verify(u => u.GetJwtSecurityHandler("token", userSession), Times.Never());
            _mockUserSessionRepository.Verify(us => us.DeactivateUserSession(userSessionId, cancellationToken), Times.Never);
        }

        /// <summary>
        /// Asserts that <see cref="RefreshTokenService.CreateRefreshToken(string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="RefreshTokenResponse"/>containig <see cref="RequestResult"/> InvalidRefreshToken 
        /// and not JWT and refreshToken is created
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task CreateRefreshToken_ShouldNotCreateRefreshToken_WhenRefreshTokenDoesNotMatchWithRequest()
        {
            //Arrange

            var clientIp = "TestClientIp";
            var userAgent = "TestUserAgent";

            var jwtSetting = new JwtSetting() { JwtSettingId = 1, HasRefresh = true, RefreshExpireMinute = 5, Secret = "iGfCtXUgK3ulAKvsNu8szl9mUeaYTRhj2EEHCfuTilZ4RKPBRh76YddLUFMxyPRHnZ3XOBvCSMLrokcS6kzpe9pdhonuIdMbXK9X" };

            var software = new Software() { SoftwareId = Guid.NewGuid(), JwtSetting = jwtSetting };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, RefreshToken = "TestRefreshToken", Software = software };

            var cancellationToken = CancellationToken.None;

            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "TestRefreshTokenFake",
                ClientIp = clientIp,
                ClientUserAgent = userAgent
            };

            _mockCurrentUserSessionService
                     .Setup(us => us.UserSession)
                     .Returns(userSession);

            //Act
            var result = await _refreshTokenService.CreateRefreshToken(refreshTokenRequest, cancellationToken);

            //Arrange
            Assert.NotNull(result);
            Assert.Equal(RequestResult.InvalidRefreshToken, result.ResultCode);
            _mockCurrentUserSessionService.Verify(us => us.UserSession, Times.Once());
            _mockUserSessionRepository.Verify(us => us.Add(It.IsAny<UserSession>(), cancellationToken), Times.Never);
            _mockJwtAuthenticationService.Verify(us => us.CreateTokenAuthenticationAsync(It.IsAny<UserSession>(), cancellationToken), Times.Never);
            _mockJwtAuthenticationService.Verify(u => u.GetJwtSecurityHandler("token", userSession), Times.Never());
            _mockUserSessionRepository.Verify(us => us.DeactivateUserSession(userSessionId, cancellationToken), Times.Never);
        }

        /// <summary>
        /// Asserts that <see cref="RefreshTokenService.CreateRefreshToken(string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="RefreshTokenResponse"/>containig <see cref="RequestResult"/> CannotRefreshBeforeExpiration 
        /// and not JWT and refreshToken is created
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task CreateRefreshToken_ShouldNotCreateRefreshToken_ReturnsResultcodeCannotRefreshBeforeExpiration_WhenCurrentDateIsLessThanValidTo()
        {
            //Arrange
            var mockHeaders = new Mock<IHeaderDictionary>();

            var jwtSetting = new JwtSetting() { JwtSettingId = 1, HasRefresh = true, RefreshExpireMinute = 5, Secret = "iGfCtXUgK3ulAKvsNu8szl9mUeaYTRhj2EEHCfuTilZ4RKPBRh76YddLUFMxyPRHnZ3XOBvCSMLrokcS6kzpe9pdhonuIdMbXK9X" };

            var software = new Software() { SoftwareId = Guid.NewGuid(), JwtSetting = jwtSetting };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, RefreshToken = "TestRefreshToken", Software = software };

            var cancellationToken = CancellationToken.None;

            var SecurityTokenClass = new SecurityTokenClass(DateTime.UtcNow.AddMinutes(5));
            var jwtSecurityHandlerResponse = new JwtSecurityHandlerResponse() { SecurityToken = SecurityTokenClass };

            var clientIp = "TestClientIp";
            var userAgent = "TestUserAgent";

            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "TestRefreshToken",
                ClientIp = clientIp,
                ClientUserAgent = userAgent
            };

            _mockCurrentUserSessionService
                .Setup(us => us.UserSession)
                .Returns(userSession);


            mockHeaders
                    .Setup(m => m.ContainsKey("X-Security-Token"))
                    .Returns(true);

            mockHeaders
                .Setup(m => m["X-Security-Token"])
                .Returns("token");

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            _mockJwtAuthenticationService
                .Setup(x => x.GetJwtSecurityHandler(It.IsAny<string>(), It.IsAny<UserSession>()))
                .Returns(jwtSecurityHandlerResponse);


            //Act
            var refreshTokenResult = await _refreshTokenService.CreateRefreshToken(refreshTokenRequest, cancellationToken);

            //Assert
            Assert.NotNull(refreshTokenResult);
            Assert.Equal(RequestResult.CannotRefreshBeforeExpiration, refreshTokenResult.ResultCode);
            _mockCurrentUserSessionService.Verify(us => us.UserSession, Times.Once());
            _mockUserSessionRepository.Verify(us => us.Add(It.IsAny<UserSession>(), cancellationToken), Times.Never());
            _mockJwtAuthenticationService.Verify(us => us.CreateTokenAuthenticationAsync(It.IsAny<UserSession>(), cancellationToken), Times.Never());
            _mockJwtAuthenticationService.Verify(u => u.GetJwtSecurityHandler("token", userSession), Times.Once());
            _mockUserSessionRepository.Verify(us => us.DeactivateUserSession(userSessionId, cancellationToken), Times.Never());
        }

        /// <summary>
        /// Asserts that <see cref="RefreshTokenService.CreateRefreshToken(string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="RefreshTokenResponse"/>containig <see cref="RequestResult"/> RefreshTokenHasBeenExpired 
        /// and not JWT and refreshToken is created
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task CreateRefreshToken_ShouldNotCreateRefreshToken_ReturnsResultcodeRefreshTokenHasBeenExpired_WhenRefreshTokenIsExpired()
        {
            //Arrange
            var mockHeaders = new Mock<IHeaderDictionary>();

            var jwtSetting = new JwtSetting() { JwtSettingId = 1, HasRefresh = true, RefreshExpireMinute = 0, Secret = "iGfCtXUgK3ulAKvsNu8szl9mUeaYTRhj2EEHCfuTilZ4RKPBRh76YddLUFMxyPRHnZ3XOBvCSMLrokcS6kzpe9pdhonuIdMbXK9X" };

            var software = new Software() { SoftwareId = Guid.NewGuid(), JwtSetting = jwtSetting };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, RefreshToken = "TestRefreshToken", Software = software };

            var cancellationToken = CancellationToken.None;

            var SecurityTokenClass = new SecurityTokenClass(DateTime.UtcNow.AddMinutes(-1));
            var jwtSecurityHandlerResponse = new JwtSecurityHandlerResponse() { SecurityToken = SecurityTokenClass };

            var clientIp = "TestClientIp";
            var userAgent = "TestUserAgent";

            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "TestRefreshToken",
                ClientIp = clientIp,
                ClientUserAgent = userAgent
            };

            _mockCurrentUserSessionService
                .Setup(us => us.UserSession)
                .Returns(userSession);


            mockHeaders
                    .Setup(m => m.ContainsKey("X-Security-Token"))
                    .Returns(true);

            mockHeaders
                .Setup(m => m["X-Security-Token"])
                .Returns("token");

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            _mockJwtAuthenticationService
                .Setup(x => x.GetJwtSecurityHandler(It.IsAny<string>(), It.IsAny<UserSession>()))
                .Returns(jwtSecurityHandlerResponse);


            //Act
            var refreshTokenResult = await _refreshTokenService.CreateRefreshToken(refreshTokenRequest, cancellationToken);

            //Assert
            Assert.NotNull(refreshTokenResult);
            Assert.Equal(RequestResult.RefreshTokenHasBeenExpired, refreshTokenResult.ResultCode);
            _mockCurrentUserSessionService.Verify(us => us.UserSession, Times.Once());
            _mockUserSessionRepository.Verify(us => us.Add(It.IsAny<UserSession>(), cancellationToken), Times.Never());
            _mockJwtAuthenticationService.Verify(us => us.CreateTokenAuthenticationAsync(It.IsAny<UserSession>(), cancellationToken), Times.Never());
            _mockJwtAuthenticationService.Verify(u => u.GetJwtSecurityHandler("token", userSession), Times.Once());
            _mockUserSessionRepository.Verify(us => us.DeactivateUserSession(userSessionId, cancellationToken), Times.Never());
        }

        /// <summary>
        /// Asserts that <see cref="RefreshTokenService.CreateRefreshToken(string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="RefreshTokenResponse"/>containig <see cref="RequestResult"/> NotAllowedThisOperation 
        /// and not JWT and refreshToken is created when Token is null 
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task CreateRefreshToken_ShouldNotCreateRefreshToken_ReturnsResultcodeNotAllowedThisOperation_WhenTokenIsNull()
        {
            //Arrange
            var mockHeaders = new Mock<IHeaderDictionary>();

            var jwtSetting = new JwtSetting() { JwtSettingId = 1, HasRefresh = true, RefreshExpireMinute = 5, Secret = "iGfCtXUgK3ulAKvsNu8szl9mUeaYTRhj2EEHCfuTilZ4RKPBRh76YddLUFMxyPRHnZ3XOBvCSMLrokcS6kzpe9pdhonuIdMbXK9X" };

            var software = new Software() { SoftwareId = Guid.NewGuid(), JwtSetting = jwtSetting };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, RefreshToken = "TestRefreshToken", Software = software };

            var cancellationToken = CancellationToken.None;


            var clientIp = "TestClientIp";
            var userAgent = "TestUserAgent";

            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "TestRefreshToken",
                ClientIp = clientIp,
                ClientUserAgent = userAgent
            };

            _mockCurrentUserSessionService
                .Setup(us => us.UserSession)
                .Returns(userSession);


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
            var refreshTokenResult = await _refreshTokenService.CreateRefreshToken(refreshTokenRequest, cancellationToken);

            //Assert
            Assert.NotNull(refreshTokenResult);
            Assert.Equal(RequestResult.NotAllowedThisOperation, refreshTokenResult.ResultCode);
            _mockCurrentUserSessionService.Verify(us => us.UserSession, Times.Once());
            _mockUserSessionRepository.Verify(us => us.Add(It.IsAny<UserSession>(), cancellationToken), Times.Never());
            _mockJwtAuthenticationService.Verify(us => us.CreateTokenAuthenticationAsync(It.IsAny<UserSession>(), cancellationToken), Times.Never());
            _mockJwtAuthenticationService.Verify(u => u.GetJwtSecurityHandler("token", userSession), Times.Never());
            _mockUserSessionRepository.Verify(us => us.DeactivateUserSession(userSessionId, cancellationToken), Times.Never());
        }

    }

    public class SecurityTokenClass : SecurityToken
    {
        private readonly DateTime _validTo;
        public SecurityTokenClass(DateTime validTo)
        {
            this._validTo = validTo;
        }
        public override DateTime ValidTo
        {
            get
            {
                return _validTo;
            }

        }
        public override string Id => throw new NotImplementedException();

        public override string Issuer => throw new NotImplementedException();

        public override SecurityKey SecurityKey => throw new NotImplementedException();

        public override SecurityKey SigningKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override DateTime ValidFrom => throw new NotImplementedException();
    }

}
