using AutoMapper;
using Entity.SSO;
using EntityServiceProvider;
using General;
using Humate.RESTApi.Areas.SSO.Structure;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Humate.Sdk.Model.SSO.Request.UserSession;
using Humate.Sdk.Model.SSO.Response.UserSession;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserSession = Humate.Sdk.Model.SSO.UserSession;

namespace Humate.RESTApi.Areas.SSO.Controllers
{
    [Route("api/sso/[controller]")]
    public class UserSessionController : SsoBaseController
    {
        public UserSessionController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        {
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<GetAllResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return base.Ok(new GetAllResponse
                {
                    UserSessions = await UnitOfWork.UserSessions.FindAllAsNoTrackingAsync<UserSession>(us =>
                         true, cancellationToken, us => us.User, us => us.Software)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("MyActiveSessions")]
        public async Task<ActionResult<MyActiveSessionsResponse>> MyActiveSessionsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (AuthenticatedUser.UserSession == null)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.NotAnyActiveSession));
                }

                return base.Ok(new MyActiveSessionsResponse
                {
                    UserSessions = await UnitOfWork.UserSessions.FindAllAsNoTrackingAsync<UserSession>(us =>
                        us.UserId == AuthenticatedUser.User.UserId && us.Status == Status.Active,
                        cancellationToken, us => us.User, us => us.Software)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetByUserId/{userId}")]
        public async Task<ActionResult<GetByUserIdResponse>> GetByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return base.Ok(new GetByUserIdResponse
                {
                    UserSessions = await UnitOfWork.UserSessions.FindAllAsNoTrackingAsync<UserSession>(us =>
                        us.UserId == userId, cancellationToken, us => us.User, us => us.Software)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetActiveByUserId/{userId}")]
        public async Task<ActionResult<GetByUserIdResponse>> GetActiveByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return base.Ok(new GetByUserIdResponse
                {
                    UserSessions = await UnitOfWork.UserSessions.FindAllAsNoTrackingAsync<UserSession>(us =>
                        us.UserId == userId && us.Status == Status.Active, cancellationToken,
                        us => us.User, us => us.Software)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetBySoftwareId/{softwareId}")]
        public async Task<ActionResult<GetBySoftwareIdResponse>> GetBySoftwareIdAsync(Guid softwareId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return base.Ok(new GetBySoftwareIdResponse
                {
                    UserSessions = await UnitOfWork.UserSessions.FindAllAsNoTrackingAsync<UserSession>(us =>
                        us.SoftwareId == softwareId, cancellationToken, us => us.User, us => us.Software)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetActiveBySoftwareId/{softwareId}")]
        public async Task<ActionResult<GetBySoftwareIdResponse>> GetActiveBySoftwareIdAsync(Guid softwareId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return base.Ok(new GetBySoftwareIdResponse
                {
                    UserSessions = await UnitOfWork.UserSessions.FindAllAsNoTrackingAsync<UserSession>(us =>
                            us.SoftwareId == softwareId && us.Status == Status.Active, cancellationToken,
                        us => us.User, us => us.Software)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPatch("Terminate")]
        public async Task<ActionResult<BaseResponse>> TerminateAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (AuthenticatedUser.UserSession == null)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.NotAnyActiveSession));
                }

                var userSession = await UnitOfWork.UserSessions.FindAsync(us =>
                    us.UserSessionId == AuthenticatedUser.UserSession.UserSessionId, cancellationToken);
                userSession.Status = Status.DeActive;
                userSession.UpdateDate = DateTime.Now.ConvertToTimestamp();
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPatch("TerminateByUserSessionId")]
        public async Task<ActionResult<BaseResponse>> TerminateByUserSessionIdAsync([FromBody]
            TerminateByUserSessionIdRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userSession = await UnitOfWork.UserSessions.FindAsync(us =>
                    us.UserSessionId == request.UserSessionId, cancellationToken);
                if (AuthenticatedUser.AccessSoftwares.All(s => s.SoftwareId != userSession.SoftwareId))
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.NotAllowedThisOperation));
                }
                userSession.Status = Status.DeActive;
                userSession.UpdateDate = DateTime.Now.ConvertToTimestamp();
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPatch("TerminateBySoftwareId")]
        public async Task<ActionResult<BaseResponse>> TerminateAsync([FromBody]
            TerminateBySoftwareIdRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var validateResult = await CheckSoftwareExistAsync(request.SoftwareId, cancellationToken);
                if (validateResult != null)
                {
                    return BadRequest(validateResult);
                }

                UnitOfWork.UserSessions.FromSqlRaw("UPDATE SSO.UserSessions "+
                    $"SET Status = 0 WHERE Status = 1 AND SoftwareId = {request.SoftwareId}");
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }
    }
}
