using AutoMapper;
using EntityServiceProvider;
using General;
using Humate.RESTApi.Areas.Identity.Structure;
using Humate.RESTApi.Infrastructure.Authentication;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Humate.Sdk.Model.Identity;
using Humate.Sdk.Model.Identity.Request.RolePermission;
using Humate.Sdk.Model.Identity.Response.RolePermission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Areas.Identity.Controllers
{
    [Route("api/identity/[controller]")]
    public class RolePermissionController : IdentityBaseController
    {
        public RolePermissionController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        { }

        [HttpGet("GetByRoleId/{roleId}")]
        public async Task<ActionResult<GetByRoleIdResponse>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var checkRoleExist = await CheckRoleExistAsync(roleId, cancellationToken);
                if (checkRoleExist != null)
                {
                    return BadRequest(checkRoleExist);
                }

                if (AuthenticatedUser.AccessRoles.All(r => r.RoleId != roleId))
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.NotAllowedThisOperation));
                }

                return Ok(new GetByRoleIdResponse
                {
                    Permissions = await UnitOfWork.RolePermissions.FindAllAsNoTrackingAsync<Permission>(s =>
                        s.RoleId == roleId, cancellationToken, s => s.Permission),
                    RoleId = roleId
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }


        [HttpGet("GetRolesUnionPermissions")]
        public async Task<ActionResult<GetRolesUnionPermissionsResponse>> GetRolesUnionPermissionsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(new GetRolesUnionPermissionsResponse
                {
                    Permissions = Mapper.Map<IEnumerable<Permission>>(await UnitOfWork.RolePermissions.GetUnionPermissionsAsync(AuthenticatedUser.User.UserId,
                        AuthenticatedUser.Software.SoftwareId, cancellationToken)),
                    Roles = Mapper.Map<IEnumerable<Role>>(AuthenticatedUser.AccessRoles)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<BaseResponse>> CreateAsync(CreateRequest request,
        CancellationToken cancellationToken = default)
        {
            try
            {
                var checkExist = await CheckRoleExistAsync(request.RoleId, cancellationToken);
                if (checkExist != null)
                {
                    return BadRequest(checkExist);
                }

                if (request.PermissionIds.Any(item =>
                        CheckPermissionExistAsync(item.GetValueOrDefault(), cancellationToken).Result != null))
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.InvalidArrayOfPermissionIds));
                }

                UnitOfWork.RolePermissions.DeletePhysicalRange(await UnitOfWork.RolePermissions
                    .FindAllAsync(rp => rp.RoleId == request.RoleId, cancellationToken));
                var rolePermissions = request.PermissionIds?
                    .Select(permissionId => new Entity.Identity.RolePermission
                    {
                        RoleId = request.RoleId,
                        PermissionId = permissionId.ToGuid(),
                        CreateDate = DateTime.Now.ConvertToTimestamp(),
                        UpdateDate = DateTime.Now.ConvertToTimestamp()
                    })
                    .ToList();
                UnitOfWork.RolePermissions.AddRange(rolePermissions);
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
