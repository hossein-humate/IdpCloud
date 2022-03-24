using IdpCloud.Common;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.EntityService.SSO;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Services
{
    /// <summary>
    /// Test suite for <see cref="CurrentSoftwareService"/>.
    /// </summary>
    public class CurrentSoftwareServiceTest
    {
        private readonly ICurrentSoftwareService _currentSoftwareService;
        private readonly Mock<ISoftwareRepository> _mockSoftwareRepository = new();
        private readonly Mock<IJwtSettingRepository> _mockJwtSettingRepository = new();
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();

        private static Guid softwareId = Guid.NewGuid();

        private readonly Dictionary<string, string> inMemorySettings = new()
        {
            { "Application:SoftwareId", softwareId.ToString() }
        };

        /// <summary>
        /// Initialises an instance of <see cref="CurrentSoftwareServiceTest"/>.
        /// </summary>
        public CurrentSoftwareServiceTest()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                                .AddInMemoryCollection(inMemorySettings)
                                .Build();

            _currentSoftwareService = new CurrentSoftwareService(
                                _mockSoftwareRepository.Object,
                                _mockJwtSettingRepository.Object,
                                configuration,
                                _mockHttpContextAccessor.Object
                );
        }

        /// <summary>
        /// Asserts than when calling <see cref="CurrentSoftwareService.Software"/>
        /// will populate the software with JWTSetting if its null and do not call JwtSetting at all
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Software_ShouldPopulateAndReturnSoftware_WhenSoftwareIsNull()
        {
            //Arrange

            var mockHeaders = new Mock<IHeaderDictionary>();
            var jwtSetting = new JwtSetting() { JwtSettingId = 1 };
            var clientId = softwareId.ToString().EncodeBase64().ToString();
            var software = new Software() { SoftwareId = softwareId, JwtSetting = jwtSetting };

            mockHeaders
                    .Setup(m => m.ContainsKey("X-ClientId"))
                    .Returns(true);

            mockHeaders
                .Setup(m => m["X-ClientId"])
                .Returns(clientId);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            _mockSoftwareRepository
                .Setup(m => m.GetSoftwareById(It.IsAny<string>(),
                 It.IsAny<CancellationToken>()))
                .ReturnsAsync(software);

            //Act

            var softwareResult = _currentSoftwareService.Software;

            //Assert
            Assert.NotNull(softwareResult);
            Assert.Equal(softwareId, softwareResult.SoftwareId);
            Assert.Equal(clientId.DecodeBase64().ToString(), softwareId.ToString());

            Assert.NotNull(softwareResult.JwtSetting);
            Assert.Equal(1, softwareResult.JwtSetting.JwtSettingId);

            mockHeaders.Verify(m => m.ContainsKey("X-ClientId"), Times.Once());
            _mockHttpContextAccessor.Verify(m => m.HttpContext, Times.Once());
            _mockSoftwareRepository.Verify(s => s.GetSoftwareById(clientId, default), Times.Once());
            _mockJwtSettingRepository.Verify(jwt => jwt.GetBySoftwareId(softwareId, default), Times.Never());
        }

        /// <summary>
        /// Asserts than when calling <see cref="CurrentSoftwareService.Software"/>
        /// will populate the software without JWTSetting if its null 
        /// and  call JwtSetting  to populate JwtSetting
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Software_ShouldPopulateAndReturnSoftwareWithJWTSetting_WhenSoftwareIsNull()
        {
            //Arrange

            var mockHeaders = new Mock<IHeaderDictionary>();
            var jwtSetting = new JwtSetting() { JwtSettingId = 1 };
            var clientId = softwareId.ToString().EncodeBase64().ToString();
            var software = new Software() { SoftwareId = softwareId };

            mockHeaders
                    .Setup(m => m.ContainsKey("X-ClientId"))
                    .Returns(true);
            mockHeaders
                .Setup(m => m["X-ClientId"])
                .Returns(clientId);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            _mockJwtSettingRepository
                .Setup(m => m.GetBySoftwareId(It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(jwtSetting);

            _mockSoftwareRepository
                .Setup(m => m.GetSoftwareById(It.IsAny<string>(),
                 It.IsAny<CancellationToken>()))
                .ReturnsAsync(software);

            //Act

            var softwareResult = _currentSoftwareService.Software;

            //Assert
            Assert.NotNull(softwareResult);
            Assert.Equal(softwareId, softwareResult.SoftwareId);
            Assert.Equal(clientId.DecodeBase64().ToString(), softwareId.ToString());

            Assert.NotNull(softwareResult.JwtSetting);
            Assert.Equal(1, softwareResult.JwtSetting.JwtSettingId);

            mockHeaders.Verify(m => m.ContainsKey("X-ClientId"), Times.Once());
            _mockSoftwareRepository.Verify(s => s.GetSoftwareById(clientId, default), Times.Once());
            _mockJwtSettingRepository.Verify(jwt => jwt.GetBySoftwareId(softwareId, default), Times.Once());
        }

        /// <summary>
        /// Asserts than when calling <see cref="CurrentSoftwareService.Software"/>
        /// should return empty/null software when header is null
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Software_ShouldReturnNull_WhenHeaderIsNull()
        {
            //Arrange
            var mockHeaders = new Mock<IHeaderDictionary>();

            _mockHttpContextAccessor
               .SetupGet(m => m.HttpContext)
               .Returns(null as HttpContext);

            //Act
            var software = _currentSoftwareService.Software;

            //Assert
            Assert.Null(software);
            mockHeaders.Verify(m => m.ContainsKey("X-ClientId"), Times.Never());
            _mockHttpContextAccessor.Verify(m => m.HttpContext, Times.Once());
            _mockSoftwareRepository.Verify(s => s.GetSoftwareById(It.IsAny<string>(), default), Times.Never());
            _mockJwtSettingRepository.Verify(jwt => jwt.GetBySoftwareId(It.IsAny<Guid>(), default), Times.Never());
        }

        /// <summary>
        /// Asserts than when calling <see cref="CurrentSoftwareService.Software"/>
        /// should return empty/null software when client id null
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Software_ShouldReturnNull_WhenClientIdIsNull()
        {
            //Arrange
            var mockHeaders = new Mock<IHeaderDictionary>();

            mockHeaders
                    .Setup(m => m.ContainsKey("X-ClientId"))
                    .Returns(true);

            mockHeaders
               .Setup(m => m["X-ClientId"])
               .Returns(null as string);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            //Act
            var software = _currentSoftwareService.Software;

            //Assert
            Assert.Null(software);
            mockHeaders.Verify(m => m.ContainsKey("X-ClientId"), Times.Once());
            _mockHttpContextAccessor.Verify(m => m.HttpContext, Times.Once());
            _mockSoftwareRepository.Verify(s => s.GetSoftwareById(It.IsAny<string>(), default), Times.Never());
            _mockJwtSettingRepository.Verify(jwt => jwt.GetBySoftwareId(It.IsAny<Guid>(), default), Times.Never());

        }

        /// <summary>
        /// Asserts than when calling <see cref="CurrentSoftwareService.Software"/>
        /// return software when it is not null
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an async unit test.</returns>
        [Fact]
        public async Task Software_ShouldReturnSoftware_WhenSoftwareIsNotNull()
        {
            //Arrange

            var mockHeaders = new Mock<IHeaderDictionary>();
            var jwtSetting = new JwtSetting() { JwtSettingId = 1 };
            var clientId = softwareId.ToString().EncodeBase64().ToString();
            var software = new Software() { SoftwareId = softwareId, JwtSetting = jwtSetting };

            mockHeaders
                    .Setup(m => m.ContainsKey("X-ClientId"))
                    .Returns(true);
            mockHeaders
                .Setup(m => m["X-ClientId"])
                .Returns(clientId);

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpContext
                .SetupGet(m => m.Request)
                .Returns(Mock.Of<HttpRequest>(m => m.Headers == mockHeaders.Object));

            _mockHttpContextAccessor
                .SetupGet(m => m.HttpContext)
                .Returns(mockHttpContext.Object);

            _mockSoftwareRepository
                .Setup(m => m.GetSoftwareById(It.IsAny<string>(),
                 It.IsAny<CancellationToken>()))
                .ReturnsAsync(software);

            //Act

            var softwareResult = _currentSoftwareService.Software;

            //Assert
            Assert.NotNull(softwareResult);
            Assert.Equal(softwareId, softwareResult.SoftwareId);
            Assert.Equal(clientId.DecodeBase64().ToString(), softwareId.ToString());

            Assert.NotNull(softwareResult.JwtSetting);
            Assert.Equal(1, softwareResult.JwtSetting.JwtSettingId);

            mockHeaders.Verify(m => m.ContainsKey("X-ClientId"), Times.Once());
            _mockHttpContextAccessor.Verify(m => m.HttpContext, Times.Once());
            _mockSoftwareRepository.Verify(s => s.GetSoftwareById(clientId, default), Times.Once());
            _mockJwtSettingRepository.Verify(jwt => jwt.GetBySoftwareId(softwareId, default), Times.Never());

            _mockHttpContextAccessor.Reset();
            _mockJwtSettingRepository.Reset();
            _mockSoftwareRepository.Reset();

            var softwareResultTwo = _currentSoftwareService.Software;

            Assert.NotNull(softwareResultTwo);
            Assert.Equal(softwareId, softwareResultTwo.SoftwareId);

            Assert.NotNull(softwareResultTwo.JwtSetting);
            Assert.Equal(1, softwareResultTwo.JwtSetting.JwtSettingId);

            _mockJwtSettingRepository.VerifyNoOtherCalls();
            _mockSoftwareRepository.VerifyNoOtherCalls();
            _mockHttpContextAccessor.VerifyNoOtherCalls();
        }
    }
}
