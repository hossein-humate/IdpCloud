using AutoMapper;
using Entity.Identity;
using EntityServiceProvider;
using General;
using Humate.RESTApi.Areas.Identity.Structure;
using Humate.RESTApi.Infrastructure.Authentication;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Humate.Sdk.Model.Identity.Request.UserRole;
using Humate.Sdk.Model.Identity.Response.UserRole;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Areas.Identity.Controllers
{
    [Route("api/identity/[controller]")]
    public class UserRoleController : IdentityBaseController
    {
        public UserRoleController(IUnitOfWork unitOfWork, IConfiguration configuration,
                IMapper mapper, IAuthenticatedUser authenticatedUser) :
                base(unitOfWork, configuration, mapper, authenticatedUser)
        {
        }

        [HttpGet("GetByUserId/{userId}")]
        public async Task<ActionResult<GetByUserIdResponse>> GetByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(new GetByUserIdResponse
                {
                    UserRoles = await UnitOfWork.UserRoles.FindAllAsNoTrackingAsync<GetByRoleId>(ur =>
                        ur.UserId == userId, cancellationToken, ur => ur.Role, ur => ur.User)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetByRoleId/{roleId}")]
        public async Task<ActionResult<GetByRoleIdResponse>> GetByRoleIdAsync(Guid roleId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(new GetByRoleIdResponse
                {
                    UserRoles = await UnitOfWork.UserRoles.FindAllAsNoTrackingAsync<GetByRoleId>(ur =>
                        ur.RoleId == roleId, cancellationToken, ur => ur.Role, ur => ur.User)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetCurrentAccessRoles")]
        public async Task<ActionResult<GetCurrentAccessRolesResponse>> GetCurrentAccessRolesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(new GetCurrentAccessRolesResponse
                {
                    AccessRoles = await UnitOfWork.UserRoles.FindAllAsNoTrackingAsync<Sdk.Model
                        .Identity.Role>(ur =>
                            ur.UserId == AuthenticatedUser.User.UserId 
                            && ur.Role.SoftwareId == AuthenticatedUser.Software.SoftwareId
                        , cancellationToken, ur => ur.Role)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<GetByRoleIdResponse>> CreateAsync(CreateRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var checkExist = await CheckRoleExistAsync(request.RoleId, cancellationToken);
                if (checkExist != null)
                {
                    return BadRequest(checkExist);
                }

                if (request.UserIds.Any(item =>
                    CheckUserExistAsync(item.GetValueOrDefault(), cancellationToken).Result != null))
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.InvalidArrayOfUserIds));
                }

                UnitOfWork.UserRoles.DeletePhysicalRange(await UnitOfWork.UserRoles
                    .FindAllAsync(rp => rp.RoleId == request.RoleId, cancellationToken));
                var usersRole = request.UserIds?
                    .Select(userId => new UserRole
                    {
                        RoleId = request.RoleId,
                        UserId = userId.ToGuid(),
                        CreateDate = DateTime.Now.ConvertToTimestamp(),
                        UpdateDate = DateTime.Now.ConvertToTimestamp()
                    })
                    .ToList();
                UnitOfWork.UserRoles.AddRange(usersRole);
                await UnitOfWork.CompleteAsync(cancellationToken);
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
