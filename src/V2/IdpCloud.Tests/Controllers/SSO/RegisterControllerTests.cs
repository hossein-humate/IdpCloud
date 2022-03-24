using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.REST.Areas.SSO.Controllers;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.Service;
using IdpCloud.ServiceProvider.Service.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
namespace IdpCloud.Tests.Controllers.SSO
{
    public class RegisterControllerTests
    {
        private readonly RegistrationController _registerController;
        private readonly Mock<IUserService> _mockUserService = new();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IConfiguration> _mockConfiguration = new();
        private readonly Mock<ICurrentUserSessionService> _mockCurrentUserSessionService = new();

        /// <summary>
        /// Initialises an instance of <see cref="RegisterControllerTests"/>.
        /// </summary>
        public RegisterControllerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetCallingAssembly()));
            var mapper = config.CreateMapper();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Proxy-Client-Remote-Ip-Address"] = "TestIp";
            httpContext.Request.Headers["User-Agent"] = "TestUserAgent";


            _registerController = new RegistrationController(
                _mockUnitOfWork.Object,
                _mockConfiguration.Object,
                 mapper,
                _mockCurrentUserSessionService.Object,
                _mockUserService.Object
                )
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };
        }
        /// <summary>
        /// Asserts that when calling <see cref="RegistrationController.RegisterAsync(RegisterRequest, CancellationToken)"/> 
        /// with a <see cref="RegisterRequest"/>
        /// containing valid details, <see cref="IUserService.RegisterAsync(RegisterRequest, string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="OkObjectResult"/> 
        /// containing the BaseResponse message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task RegisterAsync_RegisterUser()
        {
            //Arrange
            var cancellationToken = CancellationToken.None;
            var response = new BaseResponse();
            var requestResult = new RequestResult();

            var registerRequest = new RegisterRequest()
            {
                Email = "TestEmail",
                Username = "TestUsername",
                Password = "TestPassword"
            };

            _mockUserService
                .Setup(u => u.Register(It.IsAny<RegisterRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(requestResult);

            //Act
            var actionResult = await _registerController.RegisterAsync(registerRequest, cancellationToken);

            //Asserts
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var registerResponse = Assert.IsType<BaseResponse>(okObjectResult.Value);

            Assert.Equal(response.Message, registerResponse.Message);
            Assert.Equal(response.ResultCode, registerResponse.ResultCode);

            _mockUserService.Verify(m => m.Register(registerRequest, "TestIp", "TestUserAgent", cancellationToken), Times.Once());

        }

        /// <summary>
        /// Asserts that when calling <see cref="RegistrationController.RegisterAsync(RegisterRequest, CancellationToken)"/> 
        /// with a <see cref="RegisterRequest"/>
        /// containing valid details, <see cref="IUserService.RegisterAsync(RegisterRequest, string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="RequestResult"/> object and the controller will return an <see cref="BadRequestResult"/> 
        /// containing the message "Username already exist."
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task RegisterAsync_Throws400_WhenUserNameExists()
        {
            //Arrange
            var cancellationToken = CancellationToken.None;

            var response = new BaseResponse()
            {
                ResultCode = RequestResult.UsernameExist,
                Message = "Username already exist."
            };

            var requestResult = RequestResult.UsernameExist;

            var registerRequest = new RegisterRequest()
            {
                Email = "TestEmail",
                Username = "TestUsername",
                Password = "TestPassword"
            };

            _mockUserService
                .Setup(u => u.Register(It.IsAny<RegisterRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(requestResult);

            //Act
            var actionResult = await _registerController.RegisterAsync(registerRequest, cancellationToken);

            //Asserts
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var registerResponse = Assert.IsType<BaseResponse>(badRequestResult.Value);

            Assert.Equal(response.Message, registerResponse.Message);
            Assert.Equal(response.ResultCode, registerResponse.ResultCode);

            _mockUserService.Verify(m => m.Register(registerRequest, "TestIp", "TestUserAgent", cancellationToken), Times.Once());
        }

        /// <summary>
        /// Asserts that when calling <see cref="RegistrationController.RegisterAsync(RegisterRequest, CancellationToken)"/> 
        /// with a <see cref="RegisterRequest"/>
        /// containing valid details, <see cref="IUserService.RegisterAsync(RegisterRequest, string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="RequestResult"/> object and the controller will return an <see cref="StatusCodes.Status400BadRequest"/>.
        /// containing the message "Email already exist."
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task RegisterAsync_Throws400_WhenUserEmailExists()
        {
            //Arrange
            var cancellationToken = CancellationToken.None;
            var response = new BaseResponse()
            {
                ResultCode = RequestResult.EmailExist,
                Message = "Email already exist."
            };
            var requestResult = RequestResult.EmailExist;

            var registerRequest = new RegisterRequest()
            {
                Email = "TestEmail",
                Username = "TestUsername",
                Password = "TestPassword"
            };

            _mockUserService
                .Setup(u => u.Register(It.IsAny<RegisterRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(requestResult);

            //Act
            var actionResult = await _registerController.RegisterAsync(registerRequest, cancellationToken);

            //Asserts
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var registerResponse = Assert.IsType<BaseResponse>(badRequestResult.Value);

            Assert.Equal(response.Message, registerResponse.Message);
            Assert.Equal(response.ResultCode, registerResponse.ResultCode);

            _mockUserService.Verify(m => m.Register(registerRequest, "TestIp", "TestUserAgent", cancellationToken), Times.Once());
        }

        /// <summary>
        /// Asserts that when calling <see cref="RegistrationController.RegisterAsync(RegisterRequest, CancellationToken)"/> 
        /// with a <see cref="RegisterRequest"/>
        /// containing valid details, <see cref="IUserService.RegisterAsync(RegisterRequest, string, string, CancellationToken)"/>
        /// will return a null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="StatusCodes.Status500InternalServerError"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task RegisterAsync_ThrowsStatus500InternalServceError()
        {
            //Arrange
            var cancellationToken = CancellationToken.None;

            var registerRequest = new RegisterRequest()
            {
                Email = "TestEmail",
                Username = "TestUsername",
                Password = "TestPassword"
            };

            _mockUserService
                .Setup(u => u.Register(It.IsAny<RegisterRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new Exception("Exception"));

            //Act
            var actionResult = await _registerController.RegisterAsync(registerRequest, cancellationToken);

            //Asserts
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);

            _mockUserService.Verify(m => m.Register(registerRequest, "TestIp", "TestUserAgent", cancellationToken), Times.Once());
        }

        /// <summary>
        /// Asserts that when calling <see cref="RegistrationController.RegisterAsync(RegisterRequest, CancellationToken)"/> that the
        /// clientIP is passed to the <see cref="UserService.RegisterAsync(RegisterRequest, string, string, CancellationToken)"/> method
        /// from the X-Proxy-Client-Remote-Ip-Address http header or as a fall back from the remote IP address.  
        /// Also that the user agent is passed to the same from the User-Agent http header, if present.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task RegisterAsync_ClientIpAndUserAgent()
        {
            //Arrange
            var cancellationToken = CancellationToken.None;

            var registerRequest = new RegisterRequest()
            {
                Email = "TestEmail",
                Username = "TestUsername",
                Password = "TestPassword"
            };

            //Act
            var actionResult = await _registerController.RegisterAsync(registerRequest, cancellationToken);

            //Assert
            _mockUserService.Verify(m => m.Register(registerRequest, "TestIp", "TestUserAgent", cancellationToken), Times.Once());

        }

        /// <summary>
        /// Asserts that when calling <see cref="RegistrationController.ConfirmEmailAsync(string, string, CancellationToken)"/> 
        /// containing valid key and secret, 
        /// <see cref="IUserService.ConfirmEmail(string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="OkObjectResult"/> 
        /// containing the BaseResponse message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task ConfirmEmail_EmailConfirms()
        {
            //Arrange
            var cancellationToken = CancellationToken.None;
            var response = new BaseResponse();
            var requestResult = new RequestResult();
            var key = "TestKey";
            var secret = "TestSecret";


            _mockUserService
                .Setup(u => u.ConfirmEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(requestResult);

            //Act
            var actionResult = await _registerController.ConfirmEmailAsync(key, secret, cancellationToken);

            //Asserts
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var confirmEmailResponse = Assert.IsType<BaseResponse>(okObjectResult.Value);

            Assert.Equal(response.Message, confirmEmailResponse.Message);
            Assert.Equal(response.ResultCode, confirmEmailResponse.ResultCode);

            _mockUserService.Verify(m => m.ConfirmEmail(key, secret, cancellationToken), Times.Once());
        }


        /// <summary>
        /// Asserts that when calling <see cref="RegistrationController.ConfirmEmailAsync(string, string, CancellationToken)"/> 
        /// with a invalid key or invalid secret 
        /// <see cref="IUserService.ConfirmEmail(string, string, CancellationToken)"/>
        /// will return a null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="StatusCodes.Status500InternalServerError"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task ConfirmEmail_ThrowsStatus500InternalServceError()
        {
            //Arrange
            var cancellationToken = CancellationToken.None;
            var response = new BaseResponse();
            var key = "TestKey";
            var secret = "TestSecret";

            _mockUserService
                .Setup(u => u.ConfirmEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new Exception("Exception"));

            //Act
            var actionResult = await _registerController.ConfirmEmailAsync(key, secret, cancellationToken);

            //Asserts
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);

            _mockUserService.Verify(m => m.ConfirmEmail(key, secret, cancellationToken), Times.Once());
        }

        /// <summary>
        /// Asserts that when calling <see cref="RegistrationController.ConfirmEmailAsync(string, string, CancellationToken)"/> 
        /// containing valid details, <see cref="IUserService.ConfirmEmail(string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="RequestResult"/> object and the controller will return an <see cref="StatusCodes.Status400BadRequest"/>.
        /// containing the message RequestResult.NotAllowedThisOperation when user not found
        /// </summary> 
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task ConfirmEmail_Throws400_WhenUserNotExists()
        {
            //Arrange
            var mockUseRepository = new Mock<IUserRepository>();

            var cancellationToken = CancellationToken.None;
            var key = "TestKey";
            var secret = "TestSecret";
            var response = new BaseResponse()
            {
                ResultCode = RequestResult.NotAllowedThisOperation,
                Message = "You have not permission to complete this operation."
            };
            var requestResult = RequestResult.NotAllowedThisOperation;

            _mockUserService
                .Setup(u => u.ConfirmEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(requestResult);

            mockUseRepository
                .Setup(u => u.GetByEmailConfirmationSecretAndId(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as User);

            //Act
            var actionResult = await _registerController.ConfirmEmailAsync(key, secret, cancellationToken);

            //Asserts
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var confirmEmailResponse = Assert.IsType<BaseResponse>(badRequestResult.Value);

            Assert.Equal(response.Message, confirmEmailResponse.Message);
            Assert.Equal(response.ResultCode, confirmEmailResponse.ResultCode);

            _mockUserService.Verify(m => m.ConfirmEmail(key, secret, cancellationToken), Times.Once());
        }


        /// <summary>
        /// Asserts that when calling <see cref="RegistrationController.ConfirmEmailAsync(string, string, CancellationToken)"/> 
        /// containing valid details, <see cref="IUserService.ConfirmEmail(string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="RequestResult"/> object and the controller will return an <see cref="StatusCodes.Status400BadRequest"/>.
        /// containing the message RequestResult.ConfirmationCompletedBefore when email is already confirmed
        /// </summary> 
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task ConfirmEmail_Throws400_WhenEmailAlreadyConfirmed()
        {
            //Arrange
            var mockUseRepository = new Mock<IUserRepository>();

            var cancellationToken = CancellationToken.None;

            var key = "TestKey";
            var secret = "TestSecret";
            var response = new BaseResponse()
            {
                ResultCode = RequestResult.ConfirmationCompletedBefore,
                Message = "This Email Address has been confirmed before."
            };
            var requestResult = RequestResult.ConfirmationCompletedBefore;

            var user = new User { UserId = Guid.NewGuid(), EmailConfirmed = true };

            _mockUserService
                .Setup(u => u.ConfirmEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(requestResult);

            mockUseRepository
                .Setup(u => u.GetByEmailConfirmationSecretAndId(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            //Act
            var actionResult = await _registerController.ConfirmEmailAsync(key, secret, cancellationToken);

            //Asserts
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var confirmEmailResponse = Assert.IsType<BaseResponse>(badRequestResult.Value);

            Assert.Equal(response.Message, confirmEmailResponse.Message);
            Assert.Equal(response.ResultCode, confirmEmailResponse.ResultCode);

            _mockUserService.Verify(m => m.ConfirmEmail(key, secret, cancellationToken), Times.Once());
        }

        /// <summary>
        /// Asserts that when calling <see cref="RegistrationController.ConfirmEmailAsync(string, string, CancellationToken)"/> 
        /// containing valid details, <see cref="IUserService.ConfirmEmail(string, string, CancellationToken)"/>
        /// will return a non-null
        /// <see cref="RequestResult"/> object and the controller will return an <see cref="StatusCodes.Status400BadRequest"/>.
        /// containing the message RequestResult.ConfirmationLinkExpired when emailConfirmation is expired
        /// </summary> 
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task ConfirmEmail_Throws400_WhenEmailConfirmationIsExpired()
        {
            //Arrange
            var mockUseRepository = new Mock<IUserRepository>();

            var cancellationToken = CancellationToken.None;

            var key = "TestKey";
            var secret = "TestSecret";
            var response = new BaseResponse()
            {
                ResultCode = RequestResult.ConfirmationLinkExpired,
                Message = "The Email Confirmation Link has been expired, Request a new Confirmation Link or contact the support."
            };
            var requestResult = RequestResult.ConfirmationLinkExpired;

            var user = new User { UserId = Guid.NewGuid(), EmailConfirmed = false, EmailConfirmationExpiry = DateTime.UtcNow.AddMinutes(-5) };

            _mockUserService
                .Setup(u => u.ConfirmEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(requestResult);

            mockUseRepository
                .Setup(u => u.GetByEmailConfirmationSecretAndId(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            //Act
            var actionResult = await _registerController.ConfirmEmailAsync(key, secret, cancellationToken);

            //Asserts
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            var confirmEmailResponse = Assert.IsType<BaseResponse>(badRequestResult.Value);

            Assert.Equal(response.Message, confirmEmailResponse.Message);
            Assert.Equal(response.ResultCode, confirmEmailResponse.ResultCode);

            _mockUserService.Verify(m => m.ConfirmEmail(key, secret, cancellationToken), Times.Once());
        }
    }
}
