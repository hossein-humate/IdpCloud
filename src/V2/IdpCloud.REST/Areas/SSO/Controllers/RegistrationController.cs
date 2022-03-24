using AutoMapper;
using IdpCloud.REST.Areas.SSO.Structure;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Identity.Request.User;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using IdpCloud.ServiceProvider.Service.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.REST.Areas.SSO.Controllers
{
    [Route("api/sso/[controller]")]
    [ApiController]
    public class RegistrationController : SsoBaseController
    {
        private const string HeaderRemoteIpAddress = "X-Proxy-Client-Remote-Ip-Address";
        private const string HeaderUserAgent = "User-Agent";
        private readonly IUserService _userService;
        /// <summary>
        /// Instantiates a new instance of <see cref="RegistrationController"/>.
        /// </summary>
        /// <param name="unitOfWork">An instance of <see cref="IUnitOfWork"/> to inject.</param>
        /// <param name="configuration">An instance of <see cref="IConfiguration"/> to inject.</param>
        /// <param name="mapper">An instance of <see cref="IMapper"/> to inject.</param>
        /// <param name="currentUserSession">An instance of <see cref="ICurrentUserSessionService"/> to inject.</param>
        public RegistrationController(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper,
            ICurrentUserSessionService currentUserSession,
            IUserService userService)
            : base(unitOfWork, configuration, mapper, currentUserSession)
        {
            _userService = userService;
        }

        /// <summary>
        /// Controller method to Register a new user account if the provided information does not exist 
        /// and valid, this with also send an Confirmation Email Link after the registeration has been complete
        /// </summary>
        /// <param name="request">containing the acceptable parameters in this request <see cref="RegisterRequest"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns> A <see cref="Task"/> contain an <see cref="ActionResult"/> with specific RequestResult value of <see cref="BaseResponse"/> </returns>
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse>> RegisterAsync(
            [FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
        {
            ActionResult<BaseResponse> response;
            try
            {
                var clientIp = HttpContext.Request.Headers.ContainsKey(HeaderRemoteIpAddress)
                   ? HttpContext.Request.Headers[HeaderRemoteIpAddress].ToString()
                   : HttpContext.Connection.RemoteIpAddress.ToString();
                var clientUserAgent = HttpContext.Request.Headers != null && HttpContext.Request.Headers.ContainsKey(HeaderUserAgent)
                   ? HttpContext.Request.Headers[HeaderUserAgent].ToString()
                   : null;

                var result = await _userService.Register(request, clientIp, clientUserAgent, cancellationToken);
                if (result == RequestResult.RequestSuccessful)
                    response = Ok(new BaseResponse());
                else
                    response = BadRequest(BaseResponseCollection.GetBaseResponse(result));
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError,
                   BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }
            return response;
        }

        /// <summary>
        /// Controller method for Resending Email Confirmation link if the user is valid for this operation
        /// </summary>
        /// <param name="request">A <see cref="SendEmailConfirmationRequest"/> model represent the required parameters for sending email confirmation link</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel ascyn operations (optional).</param>
        /// <returns>A <see cref="Task"/> representing an async operation resulting in an <see cref="ActionResult{BaseResponse}"/> in the case of success.</returns>        
        [HttpPost("SendEmailConfirmation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse>> SendEmailConfirmationAsync(
            [FromBody] SendEmailConfirmationRequest request,
            CancellationToken cancellationToken = default)
        {
            ActionResult<BaseResponse> response;

            try
            {
                var clientIp = HttpContext.Request.Headers.ContainsKey(HeaderRemoteIpAddress)
                  ? HttpContext.Request.Headers[HeaderRemoteIpAddress].ToString()
                  : HttpContext.Connection.RemoteIpAddress.ToString();
                var clientUserAgent = HttpContext.Request.Headers != null && HttpContext.Request.Headers.ContainsKey(HeaderUserAgent)
                   ? HttpContext.Request.Headers[HeaderUserAgent].ToString()
                   : null;

                var result = await _userService.SendEmailConfirmation(request, clientIp, clientUserAgent, cancellationToken);
                if (result == RequestResult.RequestSuccessful)
                {
                    response = Ok(new BaseResponse());
                }
                else
                {
                    response = result == RequestResult.EmailUsernameOrPasswordWrong ?
                        Unauthorized(BaseResponseCollection.GetBaseResponse(result)) :
                        BadRequest(BaseResponseCollection.GetBaseResponse(result));
                }
            }
            catch
            {
                response = StatusCode(
                    StatusCodes.Status500InternalServerError,
                    BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }

            return response;
        }

        /// <summary>
        /// Controller method for Email Confirmation link if the user is valid for this operation
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="secret">The email confirmation secret</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel ascyn operations (optional).</param>
        ///  <returns>A <see cref="Task"/> representing an async operation resulting in an <see cref="ActionResult{BaseResponse}"/> in the case of success.</returns>
        [HttpGet("ConfirmEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse>> ConfirmEmailAsync(string key, string secret,
           CancellationToken cancellationToken = default)
        {
            ActionResult<BaseResponse> response;
            try
            {
                var result = await _userService.ConfirmEmail(key, secret, cancellationToken);

                if (result == RequestResult.RequestSuccessful)
                {
                    response = Ok(new BaseResponse());
                }
                else
                {
                    response = BadRequest(BaseResponseCollection.GetBaseResponse(result));
                }

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
