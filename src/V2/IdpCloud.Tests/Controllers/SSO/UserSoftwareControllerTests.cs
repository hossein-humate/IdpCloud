using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.REST.Areas.SSO.Controllers;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.SSO.Response.UserSoftware;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using IdpCloud.ServiceProvider.Service.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Controllers.SSO
{
    /// <summary>
    /// Test suite for <see cref="UserSessionController"/>.
    /// </summary>
    public class UserSoftwareControllerTests
    {
        private readonly UserSoftwareController _userSoftwareController;
        private readonly IMapper _mapper;
        private readonly Mock<IUnitOfWork> _mockUnitofWork = new();
        private readonly Mock<IConfiguration> _mockConfiguration = new();
        private readonly Mock<ICurrentUserSessionService> _mockCurrentUserSession = new();
        private readonly Mock<IUserSoftwareService> _mockUserSoftwareService = new();

        /// <summary>
        /// Initialises an instance of <see cref="UserSessionControllerTests"/>.
        /// </summary>
        public UserSoftwareControllerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(REST.Startup))));
            var mapper = config.CreateMapper();
            _mapper = mapper;

            _userSoftwareController = new UserSoftwareController(
                _mockUnitofWork.Object,
                _mockConfiguration.Object,
                mapper,
                _mockCurrentUserSession.Object,
                _mockUserSoftwareService.Object);
        }

        /// <summary>
        /// Asserts that when calling <see cref="UserSoftwareController.SoftwareListAsync(CancellationToken)"/> 
        /// that should contain a non-null
        /// <see cref="SoftwareListResponse"/> object and the controller will return an <see cref="OkObjectResult"/> containing the model.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task SoftwareListAsync_ReturnSoftwares()
        {
            //Arrange
            _mockCurrentUserSession.Setup(x => x.UserSession).Returns(new UserSession());

            var softwares = new List<Software>
            {
                new Software{ Name = "TestOne", ApiKey = "123", BusinessDescription = "DescOne" },
                new Software{ Name = "TestTwo", ApiKey = "456", BusinessDescription = "DescTwo" }
            };
            _mockUserSoftwareService
                .Setup(x => x.SoftwareList(It.IsAny<Guid>(), default))
                 .ReturnsAsync(softwares);
            var resultSoftwares = _mapper.Map<IEnumerable<SoftwareDto>>(softwares);

            //Act
            var actionResult = await _userSoftwareController.SoftwareListAsync(default);

            //Assert
            _mockCurrentUserSession.Verify(x => x.UserSession, Times.Once);

            _mockUserSoftwareService.Verify(
                m => m.SoftwareList(It.IsAny<Guid>(), default),
                Times.Once);

            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            var softwareListResponse = Assert.IsType<SoftwareListResponse>(objectResult.Value);
            Assert.Equal(RequestResult.RequestSuccessful, softwareListResponse.ResultCode);
            Assert.Equal("Request Completed Successfully.", softwareListResponse.Message);
            Assert.Equal(resultSoftwares.Count(), softwareListResponse.Softwares.Count());
            Assert.Equal(resultSoftwares.First().Name, softwareListResponse.Softwares.First().Name);
        }

        /// <summary>
        /// Asserts that when calling <see cref="UserSoftwareController.SoftwareListAsync(CancellationToken)"/> 
        /// the controller will return an <see cref="StatusCodes.Status500InternalServerError"/> response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task SoftwareListAsync_Throws()
        {
            //Arrange
            _mockCurrentUserSession.Setup(x => x.UserSession).Returns(new UserSession());

            _mockUserSoftwareService
                .Setup(x => x.SoftwareList(It.IsAny<Guid>(), default))
                 .ThrowsAsync(new Exception("Kaboom"));

            //Act
            var actionResult = await _userSoftwareController.SoftwareListAsync(default);

            //Assert
            _mockCurrentUserSession.Verify(x => x.UserSession, Times.Once);

            _mockUserSoftwareService.Verify(
                m => m.SoftwareList(It.IsAny<Guid>(), default),
                Times.Once);

            var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            var baseResponse = Assert.IsType<BaseResponse>(objectResult.Value);
            Assert.Equal(RequestResult.UnHandledException, baseResponse.ResultCode);
            Assert.Equal("Basemap Identity Server cannot handle something, Please contact for support.", baseResponse.Message);
        }
    }
}
