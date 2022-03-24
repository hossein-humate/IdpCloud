using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.Sdk.Enum;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.EntityService.SSO;
using IdpCloud.ServiceProvider.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Services
{
    /// <summary>
    /// Test suite for <see cref="JwtAuthenticationService"/>.
    /// </summary>
    public class JwtAuthenticationServiceTests
    {
        private readonly JwtAuthenticationService _jwtAuthenticationService;
        private readonly Mock<IConfiguration> _mockConfiguration = new();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<ICurrentUserSessionService> _mockCurrentUserSessionService = new();
        private readonly Mock<ICurrentSoftwareService> _mockCurrentSoftwareService = new();
        private readonly Mock<IUserSessionRepository> _mockUserSessionRepository = new();
        private readonly JwtSetting _jwtSetting = new();
        private readonly Software _software = new();
        private readonly Guid _softwareId;

        /// <summary>
        /// Initialises an instance of <see cref="JwtAuthenticationServiceTests"/> And set the authenticatedUser.
        /// </summary>
        public JwtAuthenticationServiceTests()
        {
            _jwtAuthenticationService = new JwtAuthenticationService(
            _mockUnitOfWork.Object,
            _mockCurrentUserSessionService.Object,
            _mockUserSessionRepository.Object,
            _mockCurrentSoftwareService.Object
            );

            _mockConfiguration.SetupGet(m => m["Application:AdministratorRoleId"]).Returns(Guid.NewGuid().ToString());
            _softwareId = Guid.NewGuid();
            _mockConfiguration.SetupGet(m => m["Application:SoftwareId"]).Returns(_softwareId.ToString());
        }
        /// <summary>
        /// Asserts that <see cref="JwtAuthenticationService.CreateTokenAuthenticationAsync(UserSession, CancellationToken)"/> 
        /// checks if JWTSetting has the HasRefresh property set to true So it will call
        /// <see cref="UnitOfWork.UserSessions.Update(UserSession)"/>
        /// setting hasRefresh is false then
        /// <see cref="UnitOfWork.UserSessions.Update(UserSession)"/> is not called at all.
        /// <see cref="JwtAuthenticationService.CreateTokenAuthenticationAsync(UserSession, CancellationToken)"/> will return a 
        /// <see cref="Task<string>"/> JWT token and Creates the valid JWTToken for the current user.
        /// </summary>
        /// <param name="hasRefresh">The value of the <see cref="JwtSetting.HasRefresh"/> property to use in this test.</param>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Theory]
        // when hasRefresh is true
        [InlineData(true)]
        // when hasRefresh is false
        [InlineData(false)]
        public async Task CreateTokenAuthentication_ShouldCreateJwtToken(bool hasRefresh)
        {
            //Arrange
            _jwtSetting.Issuer = "test.com";
            _jwtSetting.Audience = "test audience";
            _jwtSetting.RefreshExpireMinute = 1;
            _jwtSetting.Secret = "aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA==";
            _jwtSetting.HasRefresh = hasRefresh;
            _software.JwtSetting = _jwtSetting;

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Status = Status.Active, Software = _software };

            _mockCurrentUserSessionService
                .Setup(m => m.UserSession)
                .Returns(userSession);

            _mockCurrentSoftwareService
                .Setup(m => m.Software)
                .Returns(_software);

            _mockUserSessionRepository
                .Setup(m => m.Update(It.IsAny<UserSession>()))
                .Returns(userSession);

            _mockUnitOfWork
                .Setup(m => m.CompleteAsync(CancellationToken.None))
                .ReturnsAsync(1);

            //Act
            var resultToken = await _jwtAuthenticationService.CreateTokenAuthenticationAsync(userSession, CancellationToken.None);

            //Assert

            Assert.NotEmpty(resultToken);

            // Validate the token signiture
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(
              resultToken,
              new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  ValidAudience = _jwtSetting.Audience,
                  ValidIssuer = _jwtSetting.Issuer,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Secret))
              },
              out _);

            // Validate the claims in the payload
            var jwtSecurityToken = handler.ReadJwtToken(resultToken);
            var userSessionClaim = Assert.Single(jwtSecurityToken.Claims.Where(c => c.Type == "UserSessionId"));
            Assert.Equal(userSessionId.ToString(), userSessionClaim.Value);
            var issuerClaim = Assert.Single(jwtSecurityToken.Claims.Where(c => c.Type == "iss"));
            Assert.Equal(_jwtSetting.Issuer, issuerClaim.Value);
            var audienceClaim = Assert.Single(jwtSecurityToken.Claims.Where(c => c.Type == "aud"));
            Assert.Equal(_jwtSetting.Audience, audienceClaim.Value);

            _mockCurrentSoftwareService.Verify(m => m.Software, Times.Once());

        }

        /// <summary>
        /// Asserts that <see cref="JwtAuthenticationService.CheckTokenValidateAsync(string, CancellationToken)"/> 
        /// validates the token will return a <see cref="TokenStatus.Valid"/> JWT token status valid  for the current user.
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task CheckTokenValidate_ShouldValidateTokenAndReturnTokenStatusValid()
        {
            //Arrange

            //userSession
            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Status = Status.Active, Software = _software };

            //Create Token
            _jwtSetting.Issuer = "test.com";
            _jwtSetting.Audience = "test audience";
            _jwtSetting.RefreshExpireMinute = 1;
            _jwtSetting.Secret = "aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA==";
            _jwtSetting.HasRefresh = false;
            _software.JwtSetting = _jwtSetting;


            _mockCurrentUserSessionService
                .Setup(m => m.UserSession)
                .Returns(userSession);

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
                Audience = "test audience",
                Expires = DateTime.UtcNow.AddMinutes(5),
                Claims = claims,
                SigningCredentials = credentials
            };
            var jwtSecurityToken = handler.CreateJwtSecurityToken(SecurityTokenDescriptor);
            var token = handler.WriteToken(jwtSecurityToken);

            //Act
            var result = await _jwtAuthenticationService.CheckTokenValidateAsync(token);

            //Assert
            Assert.Equal(TokenStatus.Valid, result);
            _mockCurrentUserSessionService.Verify(us => us.UserSession, Times.Once());
        }

        /// <summary>
        /// Asserts that <see cref="JwtAuthenticationService.CheckTokenValidateAsync(string, CancellationToken)"/> 
        /// validates the token will return a <see cref="TokenStatus.Invalid"/> 
        /// if <see cref="CurrentUserSessionService.UserSession"/>is null</see>
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task CheckTokenValidate_ShouldReturnInvalidTokenStatus_WhenUserSessionIsNull()
        {
            //Arrange
            var mockUserSessionRepository = new Mock<IUserSessionRepository>();

            _jwtSetting.Issuer = "test.com";
            _jwtSetting.Audience = "test audience";
            _jwtSetting.RefreshExpireMinute = 1;
            _jwtSetting.Secret = "aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA==";
            _jwtSetting.HasRefresh = false;
            _software.JwtSetting = _jwtSetting;

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Software = _software, Status = Status.Active };

            _ = _mockCurrentUserSessionService
                .Setup(us => us.UserSession)
                .Returns(null as UserSession);

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
                Audience = "test audience",
                Expires = DateTime.UtcNow.AddMinutes(5),
                Claims = claims,
                SigningCredentials = credentials
            };
            var jwtSecurityToken = handler.CreateJwtSecurityToken(SecurityTokenDescriptor);
            var token = handler.WriteToken(jwtSecurityToken);

            //Act
            var result = await _jwtAuthenticationService.CheckTokenValidateAsync(token);

            //Assert
            Assert.Equal(TokenStatus.Invalid, result);
            _mockCurrentUserSessionService.Verify(us => us.UserSession, Times.Once());
            _mockCurrentUserSessionService.VerifyNoOtherCalls();
        }

        /// <summary>
        /// Asserts that <see cref="JwtAuthenticationService.CheckTokenValidateAsync(string, CancellationToken)"/> 
        /// validates the token will return a <see cref="TokenStatus.Invalid"/> 
        /// if <see cref="JwtSecurityHandlerResponse"/>is null</see>
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task CheckTokenValidate_ShouldReturnInvalidTokenStatus_WhenTokenIsInvalidAndHandlerIsNull()
        {
            //Arrange
            var cancellationToken = CancellationToken.None;

            _jwtSetting.Issuer = "test.com";
            _jwtSetting.Audience = "test audience";
            _jwtSetting.RefreshExpireMinute = 1;
            _jwtSetting.Secret = "aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA==";
            _jwtSetting.HasRefresh = false;
            _software.JwtSetting = _jwtSetting;

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Software = _software, Status = Status.Active };

            _ = _mockCurrentUserSessionService
               .Setup(us => us.UserSession)
               .Returns(userSession);

            //Act
            var result = await _jwtAuthenticationService.CheckTokenValidateAsync("InvalidToken", cancellationToken);

            //Assert
            Assert.Equal(TokenStatus.Invalid, result);
            _mockCurrentUserSessionService.Verify(us => us.UserSession, Times.Once());
            _mockCurrentUserSessionService.VerifyNoOtherCalls();
        }
        /// <summary>
        /// Asserts that <see cref="JwtAuthenticationService.CheckTokenValidateAsync(string, CancellationToken)"/> 
        /// validates the token will return a <see cref="TokenStatus.Invalid"/> Invalid.
        /// When token is Expired and hasRefresh is false and Deactivates the UserSession
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task CheckTokenValidate_ShouldReturnTokenStatusInvalidAndDeactivateUserSession_NoRefreshToken_WhenTokenIsExpired()
        {
            //Arrange
            var mockUserSessionRepository = new Mock<IUserSessionRepository>();
            var handler = new JwtSecurityTokenHandler();

            _jwtSetting.Issuer = "test.com";
            _jwtSetting.Audience = "test audience";
            _jwtSetting.RefreshExpireMinute = 0;
            _jwtSetting.Secret = "aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA==";
            _jwtSetting.HasRefresh = false;
            _software.JwtSetting = _jwtSetting;

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Software = _software, Status = Status.Active };

            _mockCurrentUserSessionService
                 .Setup(us => us.UserSession)
                .Returns(userSession);

            _mockUnitOfWork
               .Setup(m => m.CompleteAsync(default))
               .ReturnsAsync(1);

            IDictionary<string, object> claims = new Dictionary<string, object>
            {
                { "UserSessionId", userSessionId }
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA=="));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var SecurityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "test.com",
                Audience = "test audience",
                Expires = DateTime.UtcNow.AddSeconds(1),
                Claims = claims,
                SigningCredentials = credentials
            };
            var jwtSecurityToken = handler.CreateJwtSecurityToken(SecurityTokenDescriptor);
            var token = handler.WriteToken(jwtSecurityToken);
            Thread.Sleep(2000);

            //Act
            var resultToken = await _jwtAuthenticationService.CheckTokenValidateAsync(token);

            Assert.Equal(TokenStatus.Invalid, resultToken);

            _mockCurrentUserSessionService.Verify(m => m.UserSession, Times.Once());
            _mockUserSessionRepository.Verify(m => m.DeactivateUserSession(userSessionId, default), Times.Once());
            _mockUnitOfWork.Verify(m => m.CompleteAsync(default), Times.Once());

        }

        /// <summary>
        /// Asserts that <see cref="JwtAuthenticationService.CheckTokenValidateAsync(string, CancellationToken)"/> 
        /// validates the token will return a <see cref="TokenStatus.HasRefreshTime"/> .
        /// When token is Expired and hasRefresh is true and has RefreshExpireMinutes > than current DateTime returns HasRefreshTime token status
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task CheckTokenValidate_ShouldReturnTokenHasRefreshTime_WhenHasRefreshToken_WhenTokenIsExpired()
        {
            //Arrange
            var mockUserSessionRepository = new Mock<IUserSessionRepository>();
            var handler = new JwtSecurityTokenHandler();

            _jwtSetting.Issuer = "test.com";
            _jwtSetting.Audience = "test audience";
            _jwtSetting.RefreshExpireMinute = 5;
            _jwtSetting.Secret = "aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA==";
            _jwtSetting.HasRefresh = true;
            _software.JwtSetting = _jwtSetting;

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Software = _software };

            _mockCurrentUserSessionService
                .Setup(m => m.UserSession)
                .Returns(userSession);

            _mockUnitOfWork
                .Setup(m => m.UserSessions)
                .Returns(mockUserSessionRepository.Object);

            IDictionary<string, object> claims = new Dictionary<string, object>
            {
                { "UserSessionId", userSessionId }
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA=="));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var SecurityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "test.com",
                Audience = "test audience",
                Expires = DateTime.UtcNow.AddSeconds(1),
                Claims = claims,
                SigningCredentials = credentials
            };
            var jwtSecurityToken = handler.CreateJwtSecurityToken(SecurityTokenDescriptor);
            var token = handler.WriteToken(jwtSecurityToken);
            Thread.Sleep(2000);

            //Act
            var resultToken = await _jwtAuthenticationService.CheckTokenValidateAsync(token);

            Assert.Equal(TokenStatus.HasRefreshTime, resultToken);
            _mockCurrentUserSessionService.Verify(m => m.UserSession, Times.Once());

        }

        /// <summary>
        /// Asserts that <see cref="JwtAuthenticationService.GetJwtSecurityHandler(string)()"/>
        /// <see cref="Tuple<SecurityToken, IEnumerable<Claim>>">Returns Tuple of SecurityToken and Claims
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public void GetJwtSecurityHandler_ShouldReturnTupleOfSecurityTokenAndClaims_WhenTokenProvided()
        {
            //Arrange
            _jwtSetting.Issuer = "test.com";
            _jwtSetting.Audience = "test audience";
            _jwtSetting.RefreshExpireMinute = 1;
            _jwtSetting.Secret = "aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA==";
            _jwtSetting.HasRefresh = false;
            _software.JwtSetting = _jwtSetting;

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Software = _software };

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
                Audience = "test audience",
                Expires = DateTime.UtcNow.AddMinutes(5),
                Claims = claims,
                SigningCredentials = credentials
            };
            var jwtSecurityToken = handler.CreateJwtSecurityToken(SecurityTokenDescriptor);
            var token = handler.WriteToken(jwtSecurityToken);

            //Act
            var result = _jwtAuthenticationService.GetJwtSecurityHandler(token, userSession);

            //Assert
            Assert.Equal(SecurityTokenDescriptor.Issuer, result.SecurityToken.Issuer);
            Assert.Equal(SecurityTokenDescriptor.Audience, ((JwtSecurityToken)result.SecurityToken).Audiences.First());
            Assert.Equal(userSessionId.ToString(), ((JwtSecurityToken)result.SecurityToken).Claims.First(c => c.Type == "UserSessionId").Value);
            //Assert Claims in Tuple
            Assert.Equal(SecurityTokenDescriptor.Issuer, result.Claims.First(c => c.Type == "iss").Value);
            Assert.Equal(SecurityTokenDescriptor.Audience, result.Claims.First(c => c.Type == "aud").Value);
            Assert.Equal(userSessionId.ToString(), result.Claims.First(c => c.Type == "UserSessionId").Value);

            _mockCurrentUserSessionService.Verify(m => m.UserSession, Times.Never());
        }

        /// <summary>
        /// Asserts that <see cref="JwtAuthenticationService.GetJwtSecurityHandler(string)()"/>
        /// <see cref="JwtSecurityHandlerResponse">Returns JwtSecurityHandlerResponse 
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public void GetJwtSecurityHandler_ShouldReturnNull_WhenInvalidTokenProvided()
        {
            //Arrange
            _jwtSetting.Issuer = "test.com";
            _jwtSetting.Audience = "test audience";
            _jwtSetting.RefreshExpireMinute = 1;
            _jwtSetting.Secret = "aUdmQ3RYVWdLM3VsQUt2c051OHN6bDltVWVhWVRSaGoyRUVIQ2Z1VGlsWjRSS1BCUmg3NllkZExVRk14eVBSSG5aM1hPQnZDU01Mcm9rY1M2a3pwZTlwZGhvbnVJZE1iWEs5WA==";
            _jwtSetting.HasRefresh = false;

            _software.JwtSetting = _jwtSetting;

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession() { UserSessionId = userSessionId, Software = _software };

            //Act
            var result = _jwtAuthenticationService.GetJwtSecurityHandler("InvalidToken", userSession);

            //Assert
            Assert.Null(result);
        }
    }
}
