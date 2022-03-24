using AutoMapper;
using IdpCloud.REST.Areas.SSO.Structure;
using IdpCloud.Sdk.Model;
using IdpCloud.Sdk.Model.Security.Request.ResetPassword;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using IdpCloud.ServiceProvider.Service.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdpCloud.REST.Areas.SSO.Controllers
{
    /// <summary>
    /// Represent Controller methods and APIs for the <see cref="Entity.Security.ResetPassword"/> entity
    /// </summary>
    [Route("api/sso/[controller]")]
    public class ResetPasswordController : SsoBaseController
    {
        private const string HeaderRemoteIpAddress = "X-Proxy-Client-Remote-Ip-Address";
        private readonly IResetPasswordService _resetPasswordService;

        public ResetPasswordController(IResetPasswordService resetPasswordService, IUnitOfWork unitOfWork,
            IConfiguration configuration, IMapper mapper, ICurrentUserSessionService currentUserSession) :
            base(unitOfWork, configuration, mapper, currentUserSession)
        {
            _resetPasswordService = resetPasswordService ??
                throw new ArgumentNullException(nameof(resetPasswordService));
        }

        /// <summary>
        /// Controller method to handle Reset Password Requested by an email address by the request body
        /// </summary>
        /// <param name="email">containing the user's email address.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns> A <see cref="Task"/> contain an <see cref="ActionResult"/> with defualt value of <see cref="BaseResponse"/> </returns>
        [HttpPost("RequestByEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse>> RequestByEmailAsync(
            [FromBody] string email,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var clientIp = HttpContext.Request.Headers.ContainsKey(HeaderRemoteIpAddress)
                   ? HttpContext.Request.Headers[HeaderRemoteIpAddress].ToString()
                   : HttpContext.Connection.RemoteIpAddress.ToString();
                var clientUserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                await _resetPasswordService.RequestByEmailAsync(email, clientIp, clientUserAgent,
                    cancellationToken);

                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }
        }

        /// <summary>
        /// Controller method to handle change password, that a user requested before and provided new password in the body
        /// </summary>
        /// <param name="request">containing the acceptable parameters in this request <see cref="ChangePasswordRequest"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns> A <see cref="Task"/> contain an <see cref="ActionResult"/> with defualt value of <see cref="BaseResponse"/> </returns>
        [HttpPost("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse>> ChangePasswordAsync(
            [FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken = default)
        {
            ActionResult<BaseResponse> response;

            try
            {
                var clientIp = HttpContext.Request.Headers.ContainsKey(HeaderRemoteIpAddress)
                   ? HttpContext.Request.Headers[HeaderRemoteIpAddress].ToString()
                   : HttpContext.Connection.RemoteIpAddress.ToString();
                var clientUserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var result = await _resetPasswordService.ChangePasswordAsync(request, clientIp, clientUserAgent,
                     cancellationToken);

                if (result)
                    response = Ok(new BaseResponse());
                else
                    response = BadRequest(BaseResponseCollection
                        .GetBaseResponse(RequestResult.ResetPasswordExpiredOrInvalid));
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError,
                   BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }

            return response;
        }

        /// <summary>
        /// Controller method to handle changing password process and replace the old password with new one
        /// </summary>
        /// <param name="request">containing the acceptable parameters in this request <see cref="NewPasswordRequest"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns> A <see cref="Task"/> contain an <see cref="ActionResult"/> with defualt value of <see cref="BaseResponse"/> </returns>
        [HttpPost("NewPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse>> NewPasswordAsync(
            [FromBody] NewPasswordRequest request,
            CancellationToken cancellationToken = default)
        {
            ActionResult<BaseResponse> response;

            try
            {
                var clientIp = HttpContext.Request.Headers.ContainsKey(HeaderRemoteIpAddress)
                   ? HttpContext.Request.Headers[HeaderRemoteIpAddress].ToString()
                   : HttpContext.Connection.RemoteIpAddress.ToString();
                var clientUserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var result = await _resetPasswordService.NewPasswordAsync(request, clientIp, clientUserAgent,
                     cancellationToken);

                if (result)
                    response = Ok(new BaseResponse());
                else
                    response = BadRequest(BaseResponseCollection
                        .GetBaseResponse(RequestResult.OldPasswordWrong));
            }
            catch (Exception)
            {
                response = StatusCode(StatusCodes.Status500InternalServerError,
                   BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }

            return response;
        }

        /// <summary>
        /// Controller method to handle Decline reset password Request
        /// </summary>
        /// <param name="request">containing the acceptable parameters in this request <see cref="DeclineRequest"/>.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to trigger the cancellation of async processes.</param>
        /// <returns> A <see cref="Task"/> contain an <see cref="ActionResult"/> with defualt value of <see cref="BaseResponse"/> </returns>
        [HttpPost("Decline")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse>> DeclineAsync(
            [FromBody] DeclineRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var clientIp = HttpContext.Request.Headers.ContainsKey(HeaderRemoteIpAddress)
                   ? HttpContext.Request.Headers[HeaderRemoteIpAddress].ToString()
                   : HttpContext.Connection.RemoteIpAddress.ToString();
                var clientUserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                 await _resetPasswordService.DeclineAsync(request, clientIp, clientUserAgent,
                     cancellationToken);
                    return  Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   BaseResponseCollection.GetBaseResponse(RequestResult.UnHandledException));
            }
        }
    }
}
