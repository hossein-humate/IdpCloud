using IdpCloud.Common;
using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.ServiceProvider.EntityService.Identity;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.InMemory.Identity
{
    /// <summary>
    /// SoftwareRepository Test Suit
    /// </summary>
    public class SoftwareReposiotoryTest : TestBase
    {
        private readonly ISoftwareRepository _softwareRepository;
        private readonly EfCoreContext _efDbContext;

        public SoftwareReposiotoryTest()
        {
            _efDbContext = new EfCoreContext(GetMockContextOptions());
            _efDbContext.Database.EnsureDeleted();
            _efDbContext.Database.EnsureCreated();
            _softwareRepository = new SoftwareRepository(_efDbContext);
        }

        /// <summary>
        /// Asserts <see cref="NewSoftwareRepository.GetSoftwareById(string,CancellationToken)"/>
        /// returns Software</summary>
        /// <returns> A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task GetBySoftwareId_ShouldReturnSoftware_WhenGivenSoftwareId()
        {
            //Arrange
            var softwareId = Guid.NewGuid();
            var software = new Software() { SoftwareId = softwareId };
            await Add(software);
            var clientId = softwareId.ToString().EncodeBase64().ToString();

            //Act
            var softwareResult = await _softwareRepository.GetSoftwareById(clientId, default);

            //Assert
            Assert.NotNull(softwareResult);
            Assert.Equal(softwareId, softwareResult.SoftwareId);

        }

        private async Task Add(Software software)
        {
            await _efDbContext.AddAsync(software);
            await _efDbContext.SaveChangesAsync();
        }
    }
}
