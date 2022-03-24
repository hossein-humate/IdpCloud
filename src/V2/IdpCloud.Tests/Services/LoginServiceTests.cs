using AutoMapper;
using IdpCloud.DataProvider.Entity.BaseInfo;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.Sdk.Model.Identity.Response.User;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.EntityService.BaseInfo;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.EntityService.SSO;
using IdpCloud.ServiceProvider.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Services
{
    /// <summary>
    /// Test suite for <see cref="LoginService"/>.
    /// </summary>
    public class LoginServiceTests
    {
        private readonly LoginService _loginService;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IJwtAuthenticationService> _mockJwtAuthenticationService = new();
        private readonly Mock<IUserSessionRepository> _mockNewUserSessionRepository = new();
        private readonly Mock<ILanguageRepository> _mockNewLanguageRepository = new();
        private readonly Mock<IUserRepository> _mockNewUserRepository = new();
        private readonly Mock<ICurrentSoftwareService> _mockCurrentSoftwareService = new();

        private readonly Software _software = new();

        /// <summary>
        /// Initialises an instance of <see cref="LoginControllerTests"/>.
        /// </summary>
        public LoginServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(REST.Startup))));
            var mapper = config.CreateMapper();

            _loginService = new LoginService(
                            mapper,
                            _mockUnitOfWork.Object,
                            _mockCurrentSoftwareService.Object,
                            _mockJwtAuthenticationService.Object,
                            _mockNewUserSessionRepository.Object,
                            _mockNewLanguageRepository.Object,
                            _mockNewUserRepository.Object
                            );
        }
        /// <summary>
        /// Asserts that <see cref="LoginService.Login(LoginRequest, string, string, CancellationToken)"/>
        /// with a<see cref= "LoginRequest" />
        /// containing valid credentials will return a non-null
        /// <see cref="LoginResponse"/> 
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Theory]
        // when hasRefresh is false, emailConfirmed is true
        [InlineData(false, false)]
        public async Task Login_ShouldReturnValidLoginResponse_WhenUserIsValid(bool hasRefresh, bool emailConfirmed)
        {
            //Arrange
            var loginRequest = new LoginRequest()
            {
                UsernameOrEmail = "Test",
                Password = "TestPwd",
                LanguageId = 1
            };
            var clientIp = "TestClientIp";
            var clientAgent = "TestClientAgent";

            _software.SoftwareId = Guid.NewGuid();
            _software.JwtSetting = new JwtSetting() { JwtSettingId = 1, HasRefresh = hasRefresh };

            var userId = Guid.NewGuid();
            var organisationId = 1;
            var user = new User()
            {
                UserId = userId,
                EmailConfirmed = emailConfirmed,
                Organisation = new Organisation
                {
                    OrganisationId = organisationId,
                    Name = "TestOrganisation",
                },
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        UserId = userId,
                        Role = new Role
                        {
                            Name = "Public",
                            SoftwareId = _software.SoftwareId,
                            Software = _software
                        }
                    }
                }
            };
            var language = new Language() { LanguageId = 1 };

            _mockNewUserRepository
                .Setup(x => x.GetUserByUserNameOrEmailAndPassword(It.IsAny<string>(),
                It.IsAny<string>(), default))
                .ReturnsAsync(user);

            _mockNewLanguageRepository
                .Setup(x => x.FindById(It.IsAny<short>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(language);

            _mockJwtAuthenticationService
                .Setup(x => x.CreateTokenAuthenticationAsync(It.IsAny<UserSession>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync("token");

            _mockUnitOfWork
               .Setup(m => m.CompleteAsync(CancellationToken.None))
               .ReturnsAsync(1);

            _mockCurrentSoftwareService
                .Setup(m => m.Software)
                .Returns(_software);

            //Act
            var loginResponse = await _loginService.Login(loginRequest, clientIp, clientAgent, default);

            //Assert
            _mockNewUserRepository.Verify(m => m.GetUserByUserNameOrEmailAndPassword("Test", "TestPwd", default), Times.Once());

            if (emailConfirmed)
            {
                Assert.NotNull(loginResponse);
                Assert.Equal("token", loginResponse.Token);

                Assert.Equal(userId, loginResponse.User.UserId);
                Assert.NotNull(loginResponse.Organisation);
                Assert.Equal(organisationId, loginResponse.Organisation.OrganisationId);
                Assert.Equal("Public", loginResponse.Role.Name);
                Assert.Equal(user.Language.LanguageId, loginResponse.User.Language.LanguageId);

                _mockJwtAuthenticationService.Verify(m => m.CreateTokenAuthenticationAsync(
                    It.Is<UserSession>(us => us.UserId == userId && us.AuthType == AuthType.UserPass && us.Status == Status.Active
                    && us.Ip == clientIp && us.UserAgent == clientAgent && us.SoftwareId == _software.SoftwareId
                   ),
                    default), Times.Once());

                _mockUnitOfWork.Verify(m => m.CompleteAsync(CancellationToken.None), Times.Once());

                _mockCurrentSoftwareService.Verify(s => s.Software, Times.Exactly(2));
                _mockNewLanguageRepository.Verify(x => x.FindById(1, default), Times.Once());
            }
            else
            {
                Assert.NotNull(loginResponse);
                Assert.Equal("Before continue to login in the SSO system you need to Confirm you email address.", loginResponse.Message);
                Assert.Equal(RequestResult.EmailNotConfirmed, loginResponse.ResultCode);

                _mockJwtAuthenticationService.Verify(m => m.CreateTokenAuthenticationAsync(
                   It.Is<UserSession>(us => us.UserId == userId && us.AuthType == AuthType.UserPass && us.Status == Status.Active
                   && us.Ip == clientIp && us.UserAgent == clientAgent && us.SoftwareId == _software.SoftwareId
                  ),
                   default), Times.Never());

                _mockUnitOfWork.Verify(m => m.CompleteAsync(CancellationToken.None), Times.Never());

                _mockCurrentSoftwareService.Verify(s => s.Software, Times.Never);
                _mockNewLanguageRepository.Verify(x => x.FindById(1, default), Times.Once());
            }

            if (hasRefresh)
            {
                Assert.NotNull(loginResponse.RefreshToken);
            }
            else
            {
                Assert.Null(loginResponse.RefreshToken);
            }
        }

        /// <summary>
        /// Asserts that <see cref="LoginService.Login(LoginRequest, string, string, CancellationToken)"/>
        /// with a<see cref= "LoginRequest" />
        /// containing Invalid credentials will return a null
        /// <see cref="LoginResponse"/> 
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Login_ShouldNullLoginResponse_WhenUserIsInValid()
        {
            //Arrange
            var mockUserSessionRepository = new Mock<IUserSessionRepository>();

            var loginRequest = new LoginRequest()
            {
                UsernameOrEmail = "Test",
                Password = "TestPwd",
                LanguageId = 1
            };
            var clientIp = "TestClientIp";
            var clientAgent = "TestClientAgent";

            _mockNewUserRepository
                .Setup(x => x.GetUserByUserNameOrEmailAndPassword(It.IsAny<string>(),
                It.IsAny<string>(), default))
                .ReturnsAsync(null as User);


            //Act
            var loginResponse = await _loginService.Login(loginRequest, clientIp, clientAgent, default);

            //Assert
            Assert.Null(loginResponse);
            _mockNewUserRepository.Verify(m => m.GetUserByUserNameOrEmailAndPassword(
                loginRequest.UsernameOrEmail,
                loginRequest.Password,
                default), Times.Once());

            _mockNewLanguageRepository.Verify(m => m.FindById(It.IsAny<short>(), default), Times.Never());

            _mockJwtAuthenticationService.Verify(m => m.CreateTokenAuthenticationAsync(
                It.IsAny<UserSession>(), It.IsAny<CancellationToken>()), Times.Never());

            _mockUnitOfWork.Verify(m => m.CompleteAsync(CancellationToken.None), Times.Never());

            _mockCurrentSoftwareService.Verify(s => s.Software, Times.Never());

        }

        /// <summary>
        /// Asserts that <see cref="LoginService.Login(LoginRequest, string, string, CancellationToken)"/>
        /// with a<see cref= "LoginRequest" />
        /// containing false ConfirmEmail should return the LoginResponse with ResultCode equal EmailNotConfirmed
        /// <see cref="LoginResponse"/> 
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task Login_ShouldReturnEmailNotConfirmed_WhenUserConfirmEmailIsFalse()
        {
            //Arrange
            var loginRequest = new LoginRequest()
            {
                UsernameOrEmail = "Test",
                Password = "TestPwd",
                LanguageId = 1
            };
            var clientIp = "TestClientIp";
            var clientAgent = "TestClientAgent";

            var user = new User
            {
                EmailConfirmed = false,
                Username = "Test",
                Email = "Test@test.com"
            };
            _mockNewUserRepository
                .Setup(x => x.GetUserByUserNameOrEmailAndPassword(It.IsAny<string>(),
                It.IsAny<string>(), default))
                .ReturnsAsync(user);

            var language = new Language() { LanguageId = 1 };
            _mockNewLanguageRepository
                .Setup(x => x.FindById(It.IsAny<short>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(language);

            //Act
            var loginResponse = await _loginService.Login(loginRequest, clientIp, clientAgent, default);

            //Assert
            _mockNewUserRepository.Verify(m => m.GetUserByUserNameOrEmailAndPassword(
               loginRequest.UsernameOrEmail,
               loginRequest.Password,
               default), Times.Once());
            _mockNewLanguageRepository.Verify(x => x.FindById(1, default), Times.Once());

            _mockJwtAuthenticationService.Verify(m => m.CreateTokenAuthenticationAsync(
                It.IsAny<UserSession>(), It.IsAny<CancellationToken>()), Times.Never());

            Assert.NotNull(loginResponse);
            Assert.Equal(RequestResult.EmailNotConfirmed, loginResponse.ResultCode);
        }
    }
}
