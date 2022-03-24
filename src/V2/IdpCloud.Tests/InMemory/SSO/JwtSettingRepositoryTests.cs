

using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.ServiceProvider.EntityService.SSO;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.InMemory.SSO
{
    /// <summary>
    /// JwtSettingRepository Test Suit
    /// </summary>
    public class JwtSettingRepositoryTests : TestBase
    {
        private readonly IJwtSettingRepository _newNewJwtSettingRepository;
        private readonly EfCoreContext _efDbContext;

        public JwtSettingRepositoryTests()
        {
            _efDbContext = new EfCoreContext(GetMockContextOptions());
            _efDbContext.Database.EnsureDeleted();
            _efDbContext.Database.EnsureCreated();
            _newNewJwtSettingRepository = new JwtSettingRepository(_efDbContext);
        }

        /// <summary>
        /// Asserts <see cref="NewJwtSettingRepository.GetById(string)"/>
        /// returns JwtSetting</summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetBySoftwareId_ShouldReturnJwtSetting_WhenGivenSoftwareId()
        {
            //Arrange
            var softwareId = Guid.NewGuid();
            var jwtSettingId = 1;
            var jwtSetting = new JwtSetting() { JwtSettingId = jwtSettingId, SoftwareId = softwareId };
            await Add(jwtSetting);

            //Act
            var jwtSettingResult = await _newNewJwtSettingRepository.GetBySoftwareId(softwareId, default);

            //Assert
            Assert.NotNull(jwtSettingResult);
            Assert.Equal(jwtSettingId, jwtSetting.JwtSettingId);
            Assert.Equal(softwareId, jwtSetting.SoftwareId);
        }

        private async Task Add(JwtSetting jwtSetting)
        {
            await _efDbContext.AddAsync(jwtSetting);
            await _efDbContext.SaveChangesAsync();
        }
    }
}
