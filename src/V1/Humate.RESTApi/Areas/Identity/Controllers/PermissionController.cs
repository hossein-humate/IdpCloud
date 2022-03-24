using AutoMapper;
using Entity.Identity;
using EntityServiceProvider;
using General;
using Humate.RESTApi.Areas.Identity.Structure;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Humate.Sdk.Model.Identity.Request.Permission;
using Humate.Sdk.Model.Identity.Response.Permission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Permission = Humate.Sdk.Model.Identity.Permission;

namespace Humate.RESTApi.Areas.Identity.Controllers
{
    [Route("api/identity/[controller]")]
    public class PermissionController : IdentityBaseController
    {
        public PermissionController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        { }

        [HttpGet("GetById/{permissionId}")]
        public async Task<ActionResult<GetByIdResponse>> GetByIdAsync(Guid permissionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var checkRoleExist = await CheckPermissionExistAsync(permissionId, cancellationToken);
                if (checkRoleExist != null)
                {
                    return BadRequest(checkRoleExist);
                }

                return Ok(new GetByIdResponse
                {
                    Permission = await UnitOfWork.Permissions.FindAsync<Permission>(s =>
                        s.PermissionId == permissionId, cancellationToken)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetBySoftwareId/{softwareId}")]
        public async Task<ActionResult<GetBySoftwareIdResponse>> GetBySoftwareIdAsync(Guid softwareId, CancellationToken cancellationToken = default)
        {
            try
            {
                var permissions = await UnitOfWork.Permissions.FindAllAsync<Permission>(s =>
                       s.SoftwareId == softwareId, cancellationToken);
                if (permissions == null)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.InvalidSoftwareId));
                }
                return Ok(new GetBySoftwareIdResponse
                {
                    Permissions = permissions
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<BaseResponse>> CreateAsync([FromBody] CreateRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name) ||
                    request.SoftwareId == default || request.ParentId == default)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.RequiredNameSoftwareIdParentId));
                }

                var checkSoftwareId = await CheckSoftwareExistAsync(request.SoftwareId, cancellationToken);
                if (checkSoftwareId != null)
                {
                    return BadRequest(checkSoftwareId);
                }

                await UnitOfWork.Permissions.AddAsync(new Entity.Identity.Permission
                {
                    Name = request.Name.Trim(),
                    SoftwareId = request.SoftwareId,
                    Public = request.Public,
                    Action = request.Action,
                    Icon = request.Icon,
                    ParentId = request.ParentId,
                    SortOrder = request.SortOrder,
                    Type = (PermissionType)request.Type,
                    CreateDate = DateTime.Now.ConvertToTimestamp(),
                    UpdateDate = DateTime.Now.ConvertToTimestamp(),
                }, cancellationToken);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }

        [HttpPut("Update")]
        public async Task<ActionResult<BaseResponse>> UpdateAsync([FromBody] UpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var validateResult = ValidateForUpdatePermission(request);
                if (validateResult != null)
                {
                    return BadRequest(validateResult);
                }

                var checkPermissionExist = await CheckPermissionExistAsync(request.PermissionId, cancellationToken);
                if (checkPermissionExist != null)
                {
                    return BadRequest(checkPermissionExist);
                }

                var permission = await UnitOfWork.Permissions.FindAsync(s =>
                    s.PermissionId == request.PermissionId, cancellationToken);
                permission.Name = request.Name;
                permission.Public = request.Public;
                permission.Action = request.Action;
                permission.Icon = request.Icon;
                permission.ParentId = request.ParentId;
                permission.SortOrder = request.SortOrder;
                permission.Type = (PermissionType)request.Type;
                permission.UpdateDate = DateTime.Now.ConvertToTimestamp();
                UnitOfWork.Permissions.Update(permission);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }

        [HttpPatch("UpdateSortOrder")]
        public async Task<ActionResult<BaseResponse>> UpdateSortOrderAsync([FromBody] UpdateSortOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var item in request.Permissions)
                {
                    var checkPermissionExist = await CheckPermissionExistAsync(item.PermissionId, cancellationToken);
                    if (checkPermissionExist != null)
                    {
                        return BadRequest(checkPermissionExist);
                    }

                    var permission = await UnitOfWork.Permissions.FindAsync(s =>
                        s.PermissionId == item.PermissionId, cancellationToken);
                    permission.ParentId = item.ParentId;
                    permission.SortOrder = item.SortOrder;
                    permission.UpdateDate = DateTime.Now.ConvertToTimestamp();
                    UnitOfWork.Permissions.Update(permission);
                }

                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }

        [HttpDelete("Delete/{permissionId}")]
        public async Task<ActionResult<BaseResponse>> DeleteAsync(Guid permissionId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var checkPermissionExist = await CheckPermissionExistAsync(permissionId, cancellationToken);
                if (checkPermissionExist != null)
                {
                    return BadRequest(checkPermissionExist);
                }

                UnitOfWork.Permissions.Delete(s => s.PermissionId == permissionId);
                UnitOfWork.RolePermissions.DeleteRange(s => s.PermissionId == permissionId);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetOnRoleStateAndTreeFormat")]
        public async Task<ActionResult<GetOnRoleStateAndTreeFormatResponse>> GetOnRoleStateAndTreeFormatAsync(
            Guid softwareId, Guid roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var permissions = (await UnitOfWork.Permissions.FindAllAsync(p =>
                            p.SoftwareId == softwareId, cancellationToken,
                        p => p.RolePermissions))
                        .Select(p => new PermissionTree
                        {
                            PermissionId = p.PermissionId.ToString(),
                            ParentId = p.ParentId.ToString(),
                            Name = p.Name,
                            State = new TreeState
                            {
                                Opened = p.ParentId == Guid.Empty || (p.Childrens?.Any() ?? false),
                                Selected = p.ParentId == Guid.Empty,
                                Checked = p.RolePermissions?.Any(item =>
                                              item.RoleId == roleId) ?? false
                            }
                        });
                return Ok(new GetOnRoleStateAndTreeFormatResponse
                {
                    Permissions = permissions
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }

        #region Methods
        [NonAction]
        public BaseResponse ValidateForUpdatePermission(UpdateRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) ||
                request.PermissionId == default ||
                request.ParentId == default)
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.FillRequiredParameter);
            }
            return null;
        }
        #endregion
    }
}
