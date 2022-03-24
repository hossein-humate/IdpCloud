using AutoMapper;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Security.Request.ResetPassword;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using IdpCloud.ServiceProvider.Service;
using IdpCloud.REST.Areas.SSO.Controllers;

namespace IdpCloud.Tests.Controllers.SSO
{
    /// <summary>
    /// Test suite for <see cref="ResetPasswordController"/>.
    /// </summary>
    public class ResetPasswordControllerTests
    {
        private readonly ResetPasswordController _resetPasswordController;
        private readonly Mock<IResetPasswordService> _mockResetPasswordService = new();
        private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
        private readonly Mock<IConfiguration> _mockConfiguration = new();
        private readonly Mock<ICurrentUserSessionService> _mockCurrentUserSession = new();
        private readonly string clientIp = "TestIp";
        private readonly string clientUserAgent = "TestUserAgent";

        /// <summary>
        /// Initialises an instance of <see cref="ResetPasswordControllerTests"/>.
        /// </summary>
        public ResetPasswordControllerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetCallingAssembly()));
            var mapper = config.CreateMapper();

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Proxy-Client-Remote-Ip-Address"] = clientIp;
            httpContext.Request.Headers["User-Agent"] = clientUserAgent;

            _resetPasswordController = new ResetPasswordController(
                _mockResetPasswordService.Object,
                _mockUnitOfWork.Object,
                _mockConfiguration.Object,
                mapper,
                _mockCurrentUserSession.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };
        }

        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.RequestByEmailAsync(string, CancellationToken)"/> 
        /// with a <see cref="email"/>
        /// containing valid credentials, <see cref="ResetPasswordService.RequestByEmailAsync(string, string, string, CancellationToken)"/> 
        /// will return a non-null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task RequestByEmailAsync()
        {
            //Arrange
            var email = "TestEmail";
            var cancellationToken = CancellationToken.None;
            var response = new BaseResponse();

            //Act
            var actionResult = await _resetPasswordController.RequestByEmailAsync(email, cancellationToken);

            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var resetPasswordResponse = Assert.IsType<BaseResponse>(okObjectResult.Value);

            Assert.Equal(response.Message, resetPasswordResponse.Message);
            Assert.Equal(response.ResultCode, resetPasswordResponse.ResultCode);

            _mockResetPasswordService.Verify(m => m.RequestByEmailAsync(It.IsAny<string>(),
              It.IsAny<string>(),
              It.IsAny<string>(),
              It.IsAny<CancellationToken>()), Times.Once());

            _mockResetPasswordService.Verify(
              m => m.RequestByEmailAsync(email, "TestIp", "TestUserAgent", cancellationToken),
              Times.Once);

        }

        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.RequestByEmailAsync(string, CancellationToken)"/> 
        /// with a <see cref="email"/>
        /// invalid email, <see cref="ResetPasswordService.RequestByEmailAsync(string, string, string, CancellationToken)"/> 
        /// will return a null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="StatusCodes.Status500InternalServerError"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task RequestByPasswordAsync_Throws()
        {
            // Arange
            var email = "TestEmail";
            var cancellationToken = CancellationToken.None;

            _mockResetPasswordService
                .Setup(m => m.RequestByEmailAsync(email, It.IsAny<string>(), It.IsAny<string>(), cancellationToken))
                .ThrowsAsync(new Exception("Exception"));

            //Act
            var actionResult = await _resetPasswordController.RequestByEmailAsync(email, cancellationToken);

            //Assert
            _mockResetPasswordService.Verify(m => m.RequestByEmailAsync(email,
                It.IsAny<string>(),
                It.IsAny<string>(),
                cancellationToken), Times.Once());

            _mockResetPasswordService.Verify(m => m.RequestByEmailAsync(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once());

            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.RequestByEmailAsync(string, CancellationToken)"/> that the
        /// clientIP is passed to the <see cref="ResetPasswordService.RequestByEmailAsync(string, string, string, CancellationToken)"/> method
        /// from the X-Proxy-Client-Remote-Ip-Address http header or as a fall back from the remote IP address.  
        /// Also that the user agent is passed to the same from the User-Agent http header, if present.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task ResetPasswordAsync_ClientIpAndUserAgent()
        {
            //Arrange
            var email = "TestEmail";

            //Act
            await _resetPasswordController.RequestByEmailAsync(email, CancellationToken.None);

            _mockResetPasswordService.Verify(
                m => m.RequestByEmailAsync(It.IsAny<string>(), "TestIp", "TestUserAgent", It.IsAny<CancellationToken>()),
                Times.Once);

        }

        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.ChangePasswordAsync(ChangePasswordRequest, CancellationToken)"/> 
        /// with a <see cref="ChangePasswordRequest"/>
        /// containing acceptable parameters <see cref="ResetPasswordService.ChangePasswordAsync(ChangePasswordRequest, string, string, CancellationToken)"/> 
        /// will return a non-null
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task ChangePasswordAsync()
        {
            //Arrange
            var changePasswordRequest = new ChangePasswordRequest();
            var cancellationToken = CancellationToken.None;
            var response = new BaseResponse();

            _mockResetPasswordService
                .Setup(x => x.ChangePasswordAsync(It.IsAny<ChangePasswordRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            //Act
            var actionResult = await _resetPasswordController.ChangePasswordAsync(changePasswordRequest, cancellationToken);

            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var changePasswordResponse = Assert.IsType<BaseResponse>(okObjectResult.Value);

            Assert.Equal(response.Message, changePasswordResponse.Message);
            Assert.Equal(response.ResultCode, changePasswordResponse.ResultCode);

            _mockResetPasswordService.Verify(m => m.ChangePasswordAsync(
                It.IsAny<ChangePasswordRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once());

            _mockResetPasswordService.Verify(
             m => m.ChangePasswordAsync(changePasswordRequest, "TestIp", "TestUserAgent", cancellationToken),
             Times.Once);
        }

        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.ChangePasswordAsync(ChangePasswordRequest, CancellationToken)"/> that the
        /// clientIP is passed to the <see cref="ResetPasswordService.ChangePasswordAsync(ChangePasswordRequest, string, string, CancellationToken)"/> method
        /// from the X-Proxy-Client-Remote-Ip-Address http header or as a fall back from the remote IP address.  
        /// Also that the user agent is passed to the same from the User-Agent http header, if present.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task ChangePasswordAsync_ClientIpAndUserAgent()
        {
            //Arrange
            var changePasswordRequest = new ChangePasswordRequest();

            //Act
            await _resetPasswordController.ChangePasswordAsync(changePasswordRequest, CancellationToken.None);

            _mockResetPasswordService.Verify(
                m => m.ChangePasswordAsync(changePasswordRequest, "TestIp", "TestUserAgent", It.IsAny<CancellationToken>()),
                Times.Once);

        }

        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.ChangePasswordAsync(ChangePasswordRequest, CancellationToken)"/> 
        /// with a <see cref="ChangePasswordRequest"/>
        ///<see cref="ResetPasswordService.ChangePasswordAsync(ChangePasswordRequest, string, string, CancellationToken)"/> 
        /// will return false
        /// nd the controller will return an <see cref="StatusCodes.Status400BadRequest"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task ChangePasswordAsyncAsync_Throws_400BadRequest()
        {
            // Arange
            var changePasswordRequest = new ChangePasswordRequest();
            var cancellationToken = CancellationToken.None;
            var response = new BadRequestResult();

            _mockResetPasswordService
                .Setup(m => m.ChangePasswordAsync(changePasswordRequest, It.IsAny<string>(), It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(false);


            //Act
            var actionResult = await _resetPasswordController.ChangePasswordAsync(changePasswordRequest, cancellationToken);

            //Assert
            _mockResetPasswordService.Verify(m => m.ChangePasswordAsync(changePasswordRequest,
                It.IsAny<string>(),
                It.IsAny<string>(),
                cancellationToken), Times.Once());

            _mockResetPasswordService.Verify(m => m.ChangePasswordAsync(It.IsAny<ChangePasswordRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once());

            var objectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.ResetPasswordExpiredOrInvalid, baseResponse.ResultCode);
            Assert.Equal("Reset password Link expired or is not valid, Please try again.", baseResponse.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.ChangePasswordAsync(ChangePasswordRequest, CancellationToken)"/> 
        /// with a <see cref="ChangePasswordRequest"/>
        /// <see cref="ResetPasswordService.ChangePasswordAsync(ChangePasswordRequest, string, string, CancellationToken)"/> 
        /// will return a false
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="StatusCodes.Status500InternalServerError"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task ChangePasswordAsync_Throws_500InternalServerError()
        {
            // Arange
            var changePasswordRequest = new ChangePasswordRequest();
            var cancellationToken = CancellationToken.None;

            _mockResetPasswordService
                .Setup(m => m.ChangePasswordAsync(changePasswordRequest, It.IsAny<string>(), It.IsAny<string>(), cancellationToken))
                .ThrowsAsync(new Exception("Exception"));

            //Act
            var actionResult = await _resetPasswordController.ChangePasswordAsync(changePasswordRequest, cancellationToken);

            //Assert
            _mockResetPasswordService.Verify(m => m.ChangePasswordAsync(changePasswordRequest,
                It.IsAny<string>(),
                It.IsAny<string>(),
                cancellationToken), Times.Once());

            _mockResetPasswordService.Verify(m => m.ChangePasswordAsync(It.IsAny<ChangePasswordRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once());

            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }


        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.DeclineAsync(DeclineRequest, CancellationToken)"/> that the
        /// clientIP is passed to the <see cref="ResetPasswordService.DeclineAsync(DeclineRequest, string, string, CancellationToken)"/> method
        /// from the X-Proxy-Client-Remote-Ip-Address http header or as a fall back from the remote IP address.  
        /// Also that the user agent is passed to the same from the User-Agent http header, if present.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task DeclineAsync_ClientIpAndUserAgent()
        {
            //Arrange
            var declineRequest = new DeclineRequest();

            //Act
            await _resetPasswordController.DeclineAsync(declineRequest, CancellationToken.None);

            _mockResetPasswordService.Verify(
                m => m.DeclineAsync(declineRequest, "TestIp", "TestUserAgent", It.IsAny<CancellationToken>()),
                Times.Once);

        }
        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.DeclineAsync(DeclineRequest, CancellationToken)"/> 
        /// with a <see cref="DeclineRequest"/>
        /// containing acceptable parameters <see cref="ResetPasswordService.DeclineAsync(DeclineRequest, string, string, CancellationToken)"/> 
        /// will return a non-null
        /// <see cref="BaseResponse"/> object and the controller will return an 
        /// <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task DeclineAsync()
        {
            //Arrange
            var declineRequest = new DeclineRequest();
            var cancellationToken = CancellationToken.None;
            var response = new BaseResponse();

            //Act
            var actionResult = await _resetPasswordController.DeclineAsync(declineRequest, cancellationToken);

            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var changePasswordResponse = Assert.IsType<BaseResponse>(okObjectResult.Value);

            Assert.Equal(response.Message, changePasswordResponse.Message);
            Assert.Equal(response.ResultCode, changePasswordResponse.ResultCode);

            _mockResetPasswordService.Verify(m => m.DeclineAsync(It.IsAny<DeclineRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once());

            _mockResetPasswordService.Verify(
            m => m.DeclineAsync(declineRequest, "TestIp", "TestUserAgent", cancellationToken),
            Times.Once);
        }

        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.ChangePasswordAsync(ChangePasswordRequest, CancellationToken)"/> 
        /// with a <see cref="ChangePasswordRequest"/>
        /// <see cref="ResetPasswordService.ChangePasswordAsync(ChangePasswordRequest, string, string, CancellationToken)"/> 
        /// will return a false
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="StatusCodes.Status500InternalServerError"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task DeclineAsync_Throws_500InternalServerError()
        {
            // Arange
            var declineRequest = new DeclineRequest();
            var cancellationToken = CancellationToken.None;

            _mockResetPasswordService
                .Setup(m => m.DeclineAsync(declineRequest, It.IsAny<string>(), It.IsAny<string>(), cancellationToken))
                .ThrowsAsync(new Exception("Exception"));

            //Act
            var actionResult = await _resetPasswordController.DeclineAsync(declineRequest, cancellationToken);

            //Assert
            _mockResetPasswordService.Verify(m => m.DeclineAsync(declineRequest,
                It.IsAny<string>(),
                It.IsAny<string>(),
                cancellationToken), Times.Once());

            _mockResetPasswordService.Verify(m => m.DeclineAsync(It.IsAny<DeclineRequest>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once());

            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }


        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.NewPasswordAsync(NewPasswordRequest, CancellationToken)"/> 
        /// with a <see cref="NewPasswordRequest"/>
        /// containing acceptable parameters <see cref="ResetPasswordService.NewPasswordAsync(NewPasswordRequest, string, string, CancellationToken)"/> 
        /// true value and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task NewPasswordAsync()
        {
            //Arrange
            var request = new NewPasswordRequest();
            _mockResetPasswordService
                 .Setup(m => m.NewPasswordAsync(request, clientIp, clientUserAgent, default))
                 .ReturnsAsync(true);

            //Act
            var actionResult = await _resetPasswordController.NewPasswordAsync(request, default);

            //Assert
            _mockResetPasswordService.Verify(m => m.NewPasswordAsync(request, clientIp, clientUserAgent, default), Times.Once());
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<BaseResponse>(okObjectResult.Value);
            Assert.Equal(new BaseResponse().Message, response.Message);
            Assert.Equal(new BaseResponse().ResultCode, response.ResultCode);
        }

        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.NewPasswordAsync(NewPasswordRequest, CancellationToken)"/> 
        /// with a <see cref="ChangePasswordRequest"/>
        ///<see cref="ResetPasswordService.NewPasswordAsync(NewPasswordRequest, string, string, CancellationToken)"/> 
        /// will return false
        /// nd the controller will return an <see cref="StatusCodes.Status400BadRequest"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task NewPasswordAsync_Throws_400BadRequest()
        {
            // Arange
            var request = new NewPasswordRequest();
            _mockResetPasswordService
                .Setup(m => m.NewPasswordAsync(request, clientIp, clientUserAgent, default))
                .ReturnsAsync(false);

            //Act
            var actionResult = await _resetPasswordController.NewPasswordAsync(request, default);

            //Assert
            _mockResetPasswordService.Verify(m => m.NewPasswordAsync(request, clientIp, clientUserAgent, default), Times.Once());

            var objectResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            var response = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.OldPasswordWrong, response.ResultCode);
            Assert.Equal("The given Old Password is not correct.", response.Message);
        }

        /// <summary>
        /// Asserts that when calling <see cref="ResetPasswordController.NewPasswordAsync(NewPasswordRequest, CancellationToken)"/> 
        /// with a <see cref="NewPasswordRequest"/>
        /// <see cref="ResetPasswordService.NewPasswordAsync(NewPasswordRequest, string, string, CancellationToken)"/> 
        /// will throw exception
        /// <see cref="BaseResponse"/> object and the controller will return an <see cref="StatusCodes.Status500InternalServerError"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task NewPasswordAsync_Throws_500InternalServerError()
        {
            // Arange
            var request = new NewPasswordRequest();

            _mockResetPasswordService
                .Setup(m => m.NewPasswordAsync(request, clientIp, clientUserAgent, default))
                .ThrowsAsync(new Exception("Exception"));

            //Act
            var actionResult = await _resetPasswordController.NewPasswordAsync(request, default);

            //Assert
            _mockResetPasswordService.Verify(m => m.NewPasswordAsync(request, clientIp, clientUserAgent, default), Times.Once());
            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var response = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, response.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", response.Message);
        }
    }
}
