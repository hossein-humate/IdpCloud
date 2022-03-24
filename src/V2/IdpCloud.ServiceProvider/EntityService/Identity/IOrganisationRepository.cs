using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model.SSO.Request.Organisation;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.EntityService.Identity
{
    /// <summary>
    /// The Organisation repository, All related queries to the Organisation entity exist in this class
    /// </summary>
    public interface IOrganisationRepository
    {
        /// <summary>
        /// Add new <see cref="Organisation"/> record to the database
        /// </summary>
        /// <param name="user">Represent the record parameters data</param>
        /// <returns>Return a <see cref="Task"/>that is contain the exact <see cref="Organisation"/> record currently saved in database</returns>
        Task<Organisation> Add(Organisation Organisation);

        /// <summary>
        /// Repository method to query Database and get existing organisations in the pagination result
        /// </summary>
        /// <param name="param">A <see cref="OrganisationPaginationParam"/> param</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in
        /// an <see cref="OrganisationPaginationResult"/></returns>
        Task<OrganisationPaginationResult> GetAll(OrganisationPaginationParam param, CancellationToken cancellationToken = default);

        /// <summary>
        /// Edit the Organisation of specific <see cref="Organisation "/> record in database
        /// </summary>
        /// <param name="organisation">Represents organisation data</param>
        /// <returns>Returns a <see cref="Organisation"/> currently updated in database.</returns>
        Organisation Update(Organisation organisation);

        /// <summary>
        /// Find the Organisation by OrganisationId
        /// </summary>
        /// <param name="organisationId">Represent the <see cref="Organisation.OrganisationId"/> parameter in the entity</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <remarks>
        /// If the founded <see cref="Organisation"/> Entity contain one or more users, this method will include them inside the return result 
        /// into the <see cref="Organisation.Users"/>.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in
        /// an <see cref="Organisation"/></returns>
        Task<Organisation> FindById(int organisationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Virtual Delete it will set the DeleteDate of the provided organisation record, this is not a physical delete
        /// </summary>
        /// <param name="organisation">Represent the Organisation record that should be delete</param>
        void Delete(Organisation organisation);
    }
}
