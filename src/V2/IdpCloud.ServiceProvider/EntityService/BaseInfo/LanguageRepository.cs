using IdpCloud.DataProvider.DatabaseContext;
using IdpCloud.DataProvider.Entity.BaseInfo;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.BaseInfo
{
    /// <inheritdoc />
    public class LanguageRepository : ILanguageRepository
    {
        private readonly EfCoreContext _dbContext;
        /// <summary>
        /// Initialises an instance of <see cref="LanguageRepository"/> And inject the dbContext.
        /// </summary>
        public LanguageRepository(EfCoreContext databaseContext)
        {
            _dbContext = databaseContext;
        }

        /// <inheritdoc />
        public async Task<Language> FindById(short? languageId, CancellationToken cancellationToken = default)
        {
            var language = await _dbContext.Languages.FindAsync(new object[] { languageId }, cancellationToken);
            return language;
        }
    }
}
