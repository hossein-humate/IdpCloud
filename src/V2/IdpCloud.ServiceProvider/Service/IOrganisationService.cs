using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.SSO.Request.Organisation;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.Service
{
    /// <summary>
    /// Service to provide business logic for Organisation Entity and Organisation Controller
    /// </summary>
    public interface IOrganisationService
    {
        /// <summary>
        /// Service method for create a new Organisation
        /// </summary>
        /// <param name="request">Represent DTO of organisation from <see cref="CreateRequest"/> type</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation that contain <see cref="Organisation"/> record created by this process</returns>
        Task<Organisation> Create(CreateRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Service method get existing organisations in the pagination result
        /// </summary>
        /// <param name="paginationParam">A <see cref="OrganisationPaginationParam"/> param</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in
        /// an <see cref="OrganisationPaginationResult"/></returns>
        Task<OrganisationPaginationResult> GetAll(OrganisationPaginationParam paginationParam, CancellationToken cancellationToken = default);

        /// <summary>
        /// Service method to find Organisation and delete if there is no user inside it
        /// </summary>
        /// <param name="organisationId">Represent OrganisationId</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <remarks>
        /// If the given organisationId is Valid and not have any User, delete operation completed and return <see cref="RequestResult.RequestSuccessful"/>. 
        /// If the given organisationId is Valid and have one or more Users, delete operation not completed and return <see cref="RequestResult.OrganisationHasUser"/>.
        /// If the given organisationId is Not Valid or Not Exist, delete operation not completed and return <see cref="RequestResult.NotExistCannotContinue"/>.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in
        /// an <see cref="RequestResult"/></returns>
        Task<RequestResult> Delete(int organisationId, CancellationToken cancellationToken);

        /// <summary>
        /// Service method for editing the existing Organisation
        /// </summary>
        /// <param name="request">Represent DTO of organisation from <see cref="UpdateRequest"/> type</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns>A <see cref="Task"/> representing an async operation</returns>
        Task<Organisation> Update(UpdateRequest request, CancellationToken cancellationToken = default);
    }
}
