using IdpCloud.Common.Settings;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.ServiceProvider.EntityService.Identity;
using IdpCloud.ServiceProvider.Service.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.Services.Identity
{
    /// <summary>
    /// Test suite for <see cref="UserSoftwareService"/>.
    /// </summary>
    public class UserSoftwareServiceTests
    {
        private readonly IUserSoftwareService _userSoftwareService;
        private readonly Mock<IOptions<GlobalParameterSetting>> _mockGlobalParameter = new();
        private readonly Mock<IUserSoftwareRepository> _mockUserSoftwareRepository = new();
        private Guid userId = Guid.NewGuid();
        private Guid ssoSoftwareId = Guid.NewGuid();

        public UserSoftwareServiceTests()
        {
            _mockGlobalParameter.Setup(x => x.Value)
                .Returns(new GlobalParameterSetting { SsoSoftwareId = ssoSoftwareId });
            _userSoftwareService = new UserSoftwareService(_mockUserSoftwareRepository.Object, _mockGlobalParameter.Object);
        }

        /// <summary>
        /// Asserts that <see cref="UserSoftwareService.SoftwareList(Guid, System.Threading.CancellationToken)"/>
        /// that check the user access to any software and return the list
        /// </summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task SoftwareList_ShouldReturnSoftwares_WhenGivenUserId()
        {
            //Arrange
            var accessedSoftwares = new List<Software>()
            {
                new Software() { Name = "EVR" },
                new Software() { Name = "Basemap IdP" },
                new Software() { SoftwareId = ssoSoftwareId, Name = "Basemap IdP" }
            };
            _mockUserSoftwareRepository
                .Setup(x => x.GetSoftwareListByUserId(userId, default))
                .ReturnsAsync(accessedSoftwares);

            //Act
            var softwares = await _userSoftwareService.SoftwareList(userId, default);

            //Assert
            Assert.NotEmpty(softwares);
            Assert.NotNull(softwares);
            Assert.Equal(2, softwares.Count());
            Assert.All(softwares, item =>
            {
                Assert.Equal(SoftwareStatus.Active, item.Status);
                Assert.Null(item.DeleteDate);
                Assert.NotEqual(ssoSoftwareId, item.SoftwareId);
            });
        }
    }
}
