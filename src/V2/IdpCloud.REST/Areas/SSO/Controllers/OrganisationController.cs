using AutoMapper;
using IdpCloud.REST.Areas.SSO.Structure;
using IdpCloud.REST.Infrastructure.Attribute;
using IdpCloud.Sdk.Enum;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity;
using IdpCloud.Sdk.Model.SSO.Request.Organisation;
using IdpCloud.Sdk.Model.SSO.Response.Organisation;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.REST.Areas.SSO.Controllers
{
    /// <summary>
    /// Provide Controller methods for operations those are related to Organisation entity
    /// </summary>
    [Route("api/sso/[controller]")]
    [ApiController]
    public class OrganisationController : SsoBaseController
    {
        private readonly IOrganisationService _organisationService;

        /// <summary>
        /// Instantiates a new instance of <see cref="OrganisationController"/>.
        /// </summary>
        /// <param name="unitOfWork">An instance of <see cref="IUnitOfWork"/> to inject.</param>
        /// <param name="configuration">An instance of <see cref="IConfiguration"/> to inject.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/> to inject.</param>
        /// <param name="currentUserSession">An instance of <see cref="ICurrentUserSessionService"/> to inject.</param>
        public OrganisationController(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper,
            ICurrentUserSessionService currentUserSession,
            IOrganisationService organisationService)
            : base(unitOfWork, configuration, mapper, currentUserSession)
        {
            _organisationService = organisationService;
        }

        /// <summary>
        /// Controller method to Create a new organisation 
        /// </summary>
        /// <param name="request">containing the acceptable parameters in this request <see cref="CreateRequest"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns> A <see cref="Task"/> contain an <see cref="ActionResult"/> with specific RequestResult value of <see cref="BaseResponse"/> </returns>
        [HttpPost("Create")]
        [CheckRole(new[] { SsoRole.Administrator })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreateResponse>> CreateAsync(
            [FromBody] CreateRequest request,
            CancellationToken cancellationToken = default)
        {
            ActionResult<CreateResponse> response;
            try
            {
                var organisation = await _organisationService.Create(request, cancellationToken);
                response = Ok(new CreateResponse
                {
                    Organisation = Mapper.Map<Organisation>(organisation)
                });
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError,
                   BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }
            return response;
        }

        /// <summary>
        /// Get a paged list of organisations.
        /// </summary>
        /// <param name="pageIndex">The page number to get (defaults to 1).</param>
        /// <param name="pageSize">The number of records on each page (defaults to 100).</param>
        /// <param name="filter">Represent the filter query value.</param>
        /// <param name="orderby">Represent the orderby query value.</param>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in a resource
        /// list result of <see cref="Organisation"/>.</returns>
        [HttpGet]
        [CheckRole(new[] { SsoRole.Administrator })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ListBaseResponse<OrganisationSummary>>> Get(
            int? pageIndex,
            int? pageSize,
            string filter,
            string orderby,
            CancellationToken cancellationToken = default)
        {
            ActionResult<ListBaseResponse<OrganisationSummary>> response;
            try
            {
                int pageIndexValue = pageIndex ?? 1;
                int pageSizeValue = pageSize ?? 100;
                var organisations = await _organisationService.GetAll(new OrganisationPaginationParam()
                {
                    PageIndex = pageIndexValue,
                    PageSize = pageSizeValue,
                    Filter = filter,
                    OrderBy = orderby
                }, cancellationToken);

                response = Ok(new ListBaseResponse<OrganisationSummary>
                {
                    Resources = organisations.Items.ToList(),
                    TotalItems = organisations.TotalItems,
                    TotalPages = organisations.TotalItems % pageSizeValue == 0 ?
                        organisations.TotalItems / pageSizeValue :
                        (organisations.TotalItems / pageSizeValue) + 1
                });
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError,
                   BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }
            return response;
        }

        /// <summary>
        /// Delete the Organisation record if founded in database and has no user inside it
        /// </summary>
        /// <param name="organisationId">The organisationId to delete.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <remarks>
        /// If the given organisationId is Valid and not have any User, delete operation response contain <see cref="RequestResult.RequestSuccessful"/>. 
        /// If the given organisationId is Valid and have one or more Users, delete operation response contain <see cref="RequestResult.OrganisationHasUser"/>.
        /// If the given organisationId is Not Valid or Not Exist, delete operation response contain <see cref="RequestResult.NotExistCannotContinue"/>.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in
        /// an <see cref="BaseResponse"/></returns>
        [HttpDelete]
        [CheckRole(new[] { SsoRole.Administrator })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse>> Delete(int organisationId, CancellationToken cancellationToken = default)
        {
            ActionResult<BaseResponse> response;
            try
            {
                var result = await _organisationService.Delete(organisationId, cancellationToken);
                response = result == RequestResult.RequestSuccessful ?
                    Ok(new BaseResponse()) :
                    BadRequest(BaseResponseCollection.GetBaseResponse(result));
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError,
                   BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }
            return response;
        }

        /// <summary>
        /// Controller method to Update a organisation 
        /// </summary>
        /// <param name="request">containing the acceptable parameters in this request <see cref="UpdateRequest"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns> A <see cref="Task"/> contain an <see cref="ActionResult"/> with specific RequestResult value of <see cref="BaseResponse"/> </returns>
        [HttpPut("Update")]
        [CheckRole(new[] { SsoRole.Administrator })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UpdateResponse>> Update(
            [FromBody] UpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            ActionResult<UpdateResponse> response;
            try
            {
                var organisation = await _organisationService.Update(request, cancellationToken);
                response = Ok(new UpdateResponse
                {
                    Organisation = Mapper.Map<Organisation>(organisation)
                });
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError,
                   BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }
            return response;
        }
    }
}
