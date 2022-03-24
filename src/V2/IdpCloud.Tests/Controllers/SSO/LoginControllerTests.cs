using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.REST.Areas.SSO.Controllers;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.Sdk.Model.Identity.Response.User;
using IdpCloud.Sdk.Model.SSO.Response;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Controllers.SSO
{
    /// <summary>
    /// Test suite for <see cref="LoginController"/>.
    /// </summary>
    public class LoginControllerTests
    {
        private readonly LoginController _loginController;
        private readonly Mock<ILoginService> _mockLoginService = new();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IConfiguration> _mockConfiguration = new();
        private readonly Mock<ICurrentUserSessionService> _mockCurrentUserSession = new();
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();

        /// <summary>
        /// Initialises an instance of <see cref="LoginControllerTests"/>.
        /// </summary>
        public LoginControllerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(REST.Startup))));
            var mapper = config.CreateMapper();

            _loginController = new LoginController(
                _mockLoginService.Object,
                _mockUnitOfWork.Object,
                _mockConfiguration.Object,
                mapper,
                _mockCurrentUserSession.Object,
                _mockHttpContextAccessor.Object);
        }

        /// <summary>
        /// Asserts that when calling <see cref="LoginController.Login(LoginRequest, CancellationToken)"/> with a <see cref="LoginRequest"/>
        /// containing valid credentials, <see cref="ILoginService.Login(LoginRequest, string, string, CancellationToken)"/> will return a non-null
        /// <see cref="LoginResponse"/> object and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Login_Authenticates()
        {
            // Arange
            var request = new LoginRequest();
            var cancellationToken = CancellationToken.None;
            var response = new LoginResponse();
            _mockLoginService
                .Setup(m => m.Login(request, It.IsAny<string>(), It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(response);

            // Act
            var actionResult = await _loginController.Login(request, cancellationToken);

            // Assert
            _mockLoginService.Verify(
                m => m.Login(request, It.IsAny<string>(), It.IsAny<string>(), cancellationToken),
                Times.Once);
            _mockLoginService.Verify(
                m => m.Login(It.IsAny<LoginRequest>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var loginResponse = Assert.IsType<LoginResponse>(okObjectResult.Value);
            Assert.Equal(response, loginResponse);
        }

        /// <summary>
        /// Asserts that when calling <see cref="LoginController.Login(LoginRequest, CancellationToken)"/> with a <see cref="LoginRequest"/>
        /// containing invalid credentials, <see cref="ILoginService.Login(LoginRequest, string, string, CancellationToken)"/> will return a null
        /// <see cref="LoginResponse"/> object and the controller will return an <see cref="UnauthorizedObjectResult"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Login_Unauthorized()
        {
            // Arange
            var request = new LoginRequest();
            var cancellationToken = CancellationToken.None;
            _mockLoginService
                .Setup(m => m.Login(request, It.IsAny<string>(), It.IsAny<string>(), cancellationToken))
                .ReturnsAsync((LoginResponse?)null);

            // Act
            var actionResult = await _loginController.Login(request, cancellationToken);

            // Assert
            _mockLoginService.Verify(
                m => m.Login(request, It.IsAny<string>(), It.IsAny<string>(), cancellationToken),
                Times.Once);
            _mockLoginService.Verify(
                m => m.Login(It.IsAny<LoginRequest>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            var unauthorizedObjectResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var baseResponse = Assert.IsType<BaseResponse>(unauthorizedObjectResult.Value);
            Assert.Equal(RequestResult.EmailUsernameOrPasswordWrong, baseResponse.ResultCode);
            Assert.Equal("Email/Username or Password is not correct.", baseResponse.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="LoginController.Login(LoginRequest, CancellationToken)"/> with a <see cref="LoginRequest"/>
        /// containing invalid credentials, <see cref="ILoginService.Login(LoginRequest, string, string, CancellationToken)"/> will return a null
        /// <see cref="LoginResponse"/> object and the controller will return an <see cref="UnauthorizedObjectResult"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Login_Thorws()
        {
            // Arange
            var request = new LoginRequest();
            var cancellationToken = CancellationToken.None;
            _mockLoginService
                .Setup(m => m.Login(request, It.IsAny<string>(), It.IsAny<string>(), cancellationToken))
                .ThrowsAsync(new Exception("Kaboom"));

            // Act
            var actionResult = await _loginController.Login(request, cancellationToken);

            // Assert
            _mockLoginService.Verify(
                m => m.Login(request, It.IsAny<string>(), It.IsAny<string>(), cancellationToken),
                Times.Once);
            _mockLoginService.Verify(
                m => m.Login(It.IsAny<LoginRequest>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="LoginController.Login(LoginRequest, CancellationToken)"/> that the
        /// clientIP is passed to the <see cref="ILoginService.Login(LoginRequest, string, string, CancellationToken)"/> method
        /// from the X-Proxy-Client-Remote-Ip-Address http header or as a fall back from the remote IP address.  
        /// Also that the user agent is passed to the same from the User-Agent http header, if present.
        /// </summary>
        /// <param name="clientIpInHeader">The IP to use in the faked X-Proxy-Client-Remote-Ip-Address header - where null is passed, it will not add the header.</param>
        /// <param name="userAgent">The User Agent to use in the faked User-Agent header - where null is passed, it will not add the header.</param>
        /// <param name="expectedclientIp">The IP address to assert is passed to <see cref="ILoginService.Login(LoginRequest, string, string, CancellationToken)"/>.</param>
        /// <param name="expectedUserAgent">The User agent to assert is passed to <see cref="ILoginService.Login(LoginRequest, string, string, CancellationToken)"/>.</param>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Theory]
        // Where there is a IP in the headers, it pass that on.  It will also pass on the user agent
        [InlineData("Some other IP", "user agent", "Some other IP", "user agent")]
        // Where there is no IP in the headers, it will use the IP in the remote IP address.  It will aso pass on the user agent.
        [InlineData(null, "user agent", "192.168.1.1", "user agent")]
        // Where there is no user agent in the headers, it will pass on NULL.
        [InlineData(null, null, "192.168.1.1", null)]
        public async Task Login_ClientIpAndUserAgent(
            string clientIpInHeader,
            string userAgent,
            string expectedclientIp,
            string expectedUserAgent)
        {
            // Arange
            var mockHeaders = new Mock<IHeaderDictionary>();
            if (clientIpInHeader != null)
            {
                mockHeaders
                    .Setup(m => m.ContainsKey("X-Proxy-Client-Remote-Ip-Address"))
                    .Returns(true);
                mockHeaders
                    .Setup(m => m["X-Proxy-Client-Remote-Ip-Address"])
                    .Returns(clientIpInHeader);
            }

            if (userAgent != null)
            {
                mockHeaders
                    .Setup(m => m.ContainsKey("User-Agent"))
                    .Returns(true);
                mockHeaders
                    .Setup(m => m["User-Agent"])
                    .Returns(userAgent);
            }

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));
            _ = IPAddress.TryParse("192.168.1.1", out var remoteIpAddress);
            mockHttpContext.SetupGet(m => m.Connection)
                .Returns(Mock.Of<ConnectionInfo>(m => m.RemoteIpAddress == remoteIpAddress));
            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            _mockLoginService
                .Setup(m => m.Login(It.IsAny<LoginRequest>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new LoginResponse());

            // Act
            await _loginController.Login(new LoginRequest());

            // Assert
            _mockLoginService.Verify(
                m => m.Login(It.IsAny<LoginRequest>(), expectedclientIp, expectedUserAgent, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        /// <summary>
        /// Asserts that when calling <see cref="LoginController.Login(LoginRequest, CancellationToken)"/> with a <see cref="LoginRequest"/>
        /// containing valid credentials, <see cref="ILoginService.Login(LoginRequest, string, string, CancellationToken)"/> will return a non-null
        /// <see cref="LoginResponse"/> object and the controller will return an <see cref="BadRequestObjectResult"/> containing the model.
        /// with RequestResult EmailNotConfirmed
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Login_Throws400_WhenUserEmailNotConfirmed()
        {
            // Arange
            var request = new LoginRequest();
            var cancellationToken = CancellationToken.None;
            var response = BaseResponseCollection.GetGenericeResponse<LoginResponse>(
                RequestResult.EmailNotConfirmed);

            _mockLoginService
            .Setup(m => m.Login(request, It.IsAny<string>(), It.IsAny<string>(), cancellationToken))
            .ReturnsAsync(response);

            // Act
            var actionResult = await _loginController.Login(request, cancellationToken);

            // Assert
            _mockLoginService.Verify(
                m => m.Login(request, It.IsAny<string>(), It.IsAny<string>(), cancellationToken),
                Times.Once);
            _mockLoginService.Verify(
                m => m.Login(It.IsAny<LoginRequest>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var loginResponse = Assert.IsType<LoginResponse>(badRequestObjectResult.Value);
            Assert.Equal(response, loginResponse);
        }

        /// <summary>
        /// Asserts that when calling <see cref="LoginController.AuthUser()"/>
        /// containing valid JWT and <see cref="ICurrentUserSessionService.UserSession"/> is not null and loaded successfully
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        [Fact]
        public void AuthUser_Authenticates()
        {
            // Arange
            var response = new AuthUserResponse();

            var softwareId = Guid.NewGuid();
            var software = new Software() { SoftwareId = softwareId };
            var organisationId = 1;
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            var user = new User()
            {
                UserId = userId,
                Organisation = new Organisation
                {
                    OrganisationId = organisationId,
                    Name = "TestOrganisation"
                },
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        RoleId = roleId,
                        Role = new Role
                        {
                            RoleId = roleId,
                            Name = "Public",
                            SoftwareId = softwareId
                        }
                    }
                }
            };

            var userSessionId = Guid.NewGuid();
            var userSession = new UserSession()
            {
                UserSessionId = userSessionId,
                User = user,
                Software = software
            };
            _mockCurrentUserSession.Setup(m => m.UserSession).Returns(userSession);

            // Act
            var actionResult = _loginController.AuthUser();

            // Assert
            _mockCurrentUserSession.Verify(m => m.UserSession, Times.Exactly(4));

            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var authResponse = Assert.IsType<AuthUserResponse>(okObjectResult.Value);
            Assert.Equal("Request Completed Successfully.", authResponse.Message);
            Assert.Equal(RequestResult.RequestSuccessful, authResponse.ResultCode);
            Assert.Equal(userId, authResponse.AuthUser.User.UserId);
            Assert.Equal(userSessionId, authResponse.AuthUser.CurrentSession.UserSessionId);
            Assert.Equal(organisationId, authResponse.AuthUser.Organisation.OrganisationId);
            Assert.Equal(roleId, authResponse.AuthUser.Role.RoleId);
        }
    }
}