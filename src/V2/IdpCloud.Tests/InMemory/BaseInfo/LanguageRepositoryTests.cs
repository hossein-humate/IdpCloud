using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.BaseInfo;
using IdpCloud.ServiceProvider.EntityService.BaseInfo;
using System.Threading.Tasks;
using Xunit;

namespace IdpCloud.Tests.InMemory.BaseInfo
{
    /// <summary>
    /// Test suite for <see cref="LanguageRepository"/>.
    /// </summary>
    public class LanguageRepositoryTests : TestBase
    {
        private readonly ILanguageRepository _newLanguageReposiotry;
        private readonly EfCoreContext _efDbContext;
        /// <summary>
        /// Initialises an instance of <see cref="LanguageRepositoryTests"/> And set the authenticatedUser.
        /// </summary>
        public LanguageRepositoryTests()
        {
            _efDbContext = new EfCoreContext(GetMockContextOptions());
            _efDbContext.Database.EnsureDeleted();
            _efDbContext.Database.EnsureCreated();
            _newLanguageReposiotry = new LanguageRepository(_efDbContext);
        }
        /// <summary>
        /// Asserts <see cref="LanguageRepository.FindById(short?, System.Threading.CancellationToken)"/>
        /// returns Language
        /// </summary>
        /// <returns>A <see cref="Task"/> reprsenting as async opertation</returns>
        [Fact]
        public async Task FindById_ShouldReturnLanguage_WhenGivenValidLanguageId()
        {
            //Arrange
            short _languageId = 123;
            AddLanguage(_languageId);

            //Act
            var language = await _newLanguageReposiotry.FindById(_languageId, default);

            //Assert
            Assert.NotNull(language);
        }

        private async void AddLanguage(short languageId)
        {
            var language = new Language() { LanguageId = languageId };
            await _efDbContext.AddAsync(language);
            await _efDbContext.SaveChangesAsync();
        }
        internal void Dispose()
        {
            _efDbContext.Dispose();
        }
    }
}
