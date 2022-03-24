using AutoMapper;
using IdpCloud.REST.Areas.Identity.Structure;
using IdpCloud.REST.Infrastructure.Attribute;
using IdpCloud.Sdk.Enum;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.Sdk.Model.Identity.Response.User;
using IdpCloud.Sdk.Model.SSO.Request.User;
using IdpCloud.Sdk.Model.SSO.Response.Organisation;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using IdpCloud.ServiceProvider.Service.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.REST.Areas.Identity.Controllers
{
    /// <summary>
    /// Provide Controller methods for operations those are related to User
    /// </summary>
    [Route("api/identity/[controller]")]
    [ApiController]
    public class NewUserController : IdentityBaseController
    {
        private readonly IUserService _userService;
        public NewUserController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper,
            ICurrentUserSessionService currentUserSession,
            IUserService userService)
             : base(unitOfWork, configuration, mapper, currentUserSession)
        {
            _userService = userService;
        }

        /// <summary>
        /// Controller method to Create a new User 
        /// </summary>
        /// <param name="request">containing the acceptable parameters in this request <see cref="CreateRequest"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns> A <see cref="Task"/> contain an <see cref="ActionResult"/> with specific RequestResult value of <see cref="BaseResponse"/> </returns>
        [HttpPost("Create")]
        [CheckRole(new[] { SsoRole.Administrator })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreateUserResponse>> CreateAsync(
            [FromBody] CreateUserRequest request,
            CancellationToken cancellationToken = default)
        {
            ActionResult<CreateUserResponse> response;
            try
            {
                var user = await _userService.Create(request, cancellationToken);
                response = Ok(new CreateUserResponse
                {
                    User = Mapper.Map<NewUser>(user)
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
        /// Represent a paged list of users and functionality for clients to pass custom filter and orderby queries to the controller method.
        /// </summary>
        /// <param name="pageIndex">The page number to get (defaults to 1).</param>
        /// <param name="pageSize">The number of records on each page (defaults to 100).</param>
        /// <param name="filter">Represent the filter query value.</param>
        /// <param name="orderby">Represent the orderby query value.</param>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in a resource
        /// list result of <see cref="User"/>.</returns>
        [HttpGet]
        [CheckRole(new[] { SsoRole.Administrator })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ListBaseResponse<UserSummary>>> Get(
            int? pageIndex,
            int? pageSize,
            string filter,
            string orderby,
            CancellationToken cancellationToken = default)
        {
            ActionResult<ListBaseResponse<UserSummary>> response;
            try
            {
                int pageIndexValue = pageIndex ?? 1;
                int pageSizeValue = pageSize ?? 100;
                var users = await _userService.GetAll(new UserPaginationParam()
                {
                    PageIndex = pageIndexValue,
                    PageSize = pageSizeValue,
                    Filter = filter,
                    OrderBy = orderby
                }, cancellationToken);

                response = Ok(new ListBaseResponse<UserSummary>
                {
                    Resources = users.Items.ToList(),
                    TotalItems = users.TotalItems,
                    TotalPages = users.TotalItems % pageSizeValue == 0 ?
                        users.TotalItems / pageSizeValue :
                        (users.TotalItems / pageSizeValue) + 1
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
        /// Controller method to Update a user 
        /// </summary>
        /// <param name="request">containing the acceptable parameters in this request <see cref="UpdateUserRequest"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns> A <see cref="Task"/> contain an <see cref="ActionResult"/> with specific RequestResult value of <see cref="BaseResponse"/> </returns>
        [HttpPut("Update")]
        [CheckRole(new[] { SsoRole.Administrator })]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UpdateUserResponse>> Update(
            [FromBody] UpdateUserRequest request,
            CancellationToken cancellationToken = default)
        {
            ActionResult<UpdateUserResponse> response;
            try
            {
                var user = await _userService.Update(request, cancellationToken);
                response = Ok(new UpdateUserResponse
                {
                    User = Mapper.Map<NewUser>(user)
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
