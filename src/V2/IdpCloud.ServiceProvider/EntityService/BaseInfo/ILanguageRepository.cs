using IdpCloud.DataProvider.Entity.BaseInfo;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.BaseInfo
{
    /// <summary>
    /// The Language repository
    /// </summary>
    public interface ILanguageRepository
    {
        /// <summary>
        /// Get Language by Id
        /// </summary>
        /// <param name="languageId">The <see cref="Language.LanguageId"/>LanguageId</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>The<see cref="Language"/>Language entity</returns>
        Task<Language> FindById(short? languageId, CancellationToken cancellationToken = default);
    }
}
