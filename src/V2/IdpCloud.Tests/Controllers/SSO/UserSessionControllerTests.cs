using AutoMapper;
using IdpCloud.REST.Areas.SSO.Controllers;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.SSO.Request.UserSession;
using IdpCloud.Sdk.Model.SSO.Response.UserSession;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
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
    /// <summary>
    /// Test suite for <see cref="UserSessionController"/>.
    /// </summary>
    public class UserSessionControllerTests
    {
        private readonly UserSessionController _userSessionController;
        private readonly Mock<IRefreshTokenService> _mockRefreshTokenService = new();
        private readonly Mock<IUnitOfWork> _mockUnitofWork = new();
        private readonly Mock<IConfiguration> _mockConfiguration = new();
        private readonly Mock<ICurrentUserSessionService> _mockCurrentUserSession = new();

        /// <summary>
        /// Initialises an instance of <see cref="UserSessionControllerTests"/>.
        /// </summary>
        public UserSessionControllerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetCallingAssembly()));
            var mapper = config.CreateMapper();

            _userSessionController = new UserSessionController(
                _mockUnitofWork.Object,
                _mockConfiguration.Object,
                mapper,
                _mockCurrentUserSession.Object,
                _mockRefreshTokenService.Object);
        }
        /// <summary>
        /// Asserts that when calling <see cref="UserSessionController.RefreshTokenAsync(RefreshTokenRequest, CancellationToken)"/>
        /// with a <see cref="RefreshTokenRequest"/>
        /// containing valid details <see cref="IRefreshTokenService.CreateRefreshToken(RefreshTokenRequest, CancellationToken)"/> 
        /// will return a non-null
        /// <see cref="RefreshTokenResponse"/> object and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task RefreshTokenAsync_CreatesRefreshToken()
        {
            //Arrange
            var clientIp = "TestClientIp";
            var userAgent = "TestUserAgent";
            var cancellationToken = CancellationToken.None;

            var refreshTokenRequest = new RefreshTokenRequest
            {
                RefreshToken = "TestRefreshToken",
                ClientIp = clientIp,
                ClientUserAgent = userAgent
            };

            var refreshTokenResponse = new RefreshTokenResponse()
            {
                Token = "testToken",
                RefreshToken = "TestRefreshToken"
            };

            _mockRefreshTokenService
                .Setup(x => x.CreateRefreshToken(It.IsAny<RefreshTokenRequest>(), cancellationToken))
                .ReturnsAsync(refreshTokenResponse);

            //Act

            var actionResult = await _userSessionController.RefreshTokenAsync(refreshTokenRequest, cancellationToken);

            //Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var refreshTokenResponseResult = Assert.IsType<RefreshTokenResponse>(okObjectResult.Value);

            _mockRefreshTokenService.Verify(
               m => m.CreateRefreshToken(refreshTokenRequest, cancellationToken),
               Times.Once);
            _mockRefreshTokenService.Verify(
                m => m.CreateRefreshToken(It.IsAny<RefreshTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);

            Assert.Equal(refreshTokenResponse, refreshTokenResponseResult);
        }


        /// <summary>
        /// Asserts that when calling <see cref="UserSessionController.RefreshTokenAsync(RefreshTokenRequest, CancellationToken))"/> 
        /// with a <see cref="RefreshTokenRequest"/>
        /// containing invalid details, <see cref="IRefreshTokenService.CreateRefreshToken(RefreshTokenRequest, CancellationToken)"/> will return a null
        /// <see cref="RefreshTokenResponse"/> object and the controller will return an <see cref="StatusCodes.Status500InternalServerError"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task RefreshTokenAsync_Throws()
        {
            //Arrange
            var refreshTokenRequest = new RefreshTokenRequest();
            var cancellationToken = CancellationToken.None;

            _mockRefreshTokenService
                .Setup(x => x.CreateRefreshToken(It.IsAny<RefreshTokenRequest>(), cancellationToken))
                 .ThrowsAsync(new Exception("Kaboom"));

            //Act
            var actionResult = await _userSessionController.RefreshTokenAsync(refreshTokenRequest, cancellationToken);

            //Assert
            _mockRefreshTokenService.Verify(
                m => m.CreateRefreshToken(refreshTokenRequest, cancellationToken),
                Times.Once);

            _mockRefreshTokenService.Verify(
                m => m.CreateRefreshToken(It.IsAny<RefreshTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Once);

            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }
    }
}
