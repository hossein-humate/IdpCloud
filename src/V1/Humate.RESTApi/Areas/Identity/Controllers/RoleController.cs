using AutoMapper;
using EntityServiceProvider;
using General;
using Humate.RESTApi.Areas.Identity.Structure;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Humate.Sdk.Model.Identity;
using Humate.Sdk.Model.Identity.Request.Role;
using Humate.Sdk.Model.Identity.Response.Role;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Areas.Identity.Controllers
{
    [Route("api/identity/[controller]")]
    public class RoleController : IdentityBaseController
    {
        public RoleController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        { }

        [HttpGet("GetById/{roleId}")]
        public async Task<ActionResult<GetByIdResponse>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var checkRoleExist = await CheckRoleExistAsync(roleId, cancellationToken);
                if (checkRoleExist != null)
                {
                    return BadRequest(checkRoleExist);
                }

                return Ok(new GetByIdResponse
                {
                    Role = await UnitOfWork.Roles.FindAsync<Role>(s =>
                        s.RoleId == roleId, cancellationToken)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetBySoftwareId/{softwareId}")]
        public async Task<ActionResult<GetBySoftwareIdResponse>> GetBySoftwareIdAsync(Guid softwareId, CancellationToken cancellationToken = default)
        {
            try
            {
                var roles = await UnitOfWork.Roles.FindAllAsync<Role>(s =>
                       s.SoftwareId == softwareId, cancellationToken);
                if (roles == null)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.InvalidSoftwareId));
                }
                return Ok(new GetBySoftwareIdResponse
                {
                    Roles = roles
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<BaseResponse>> CreateAsync([FromBody] CreateRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name) ||
                    request.SoftwareId == default)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.FillRequiredParameter));
                }

                var checkSoftwareId = await CheckSoftwareExistAsync(request.SoftwareId, cancellationToken);
                if (checkSoftwareId != null)
                {
                    return BadRequest(checkSoftwareId);
                }

                var validateResult = await ValidateForCreateRoleAsync(request, cancellationToken);
                if (validateResult != null)
                {
                    return BadRequest(validateResult);
                }
                await UnitOfWork.Roles.AddAsync(new Entity.Identity.Role
                {
                    Name = request.Name.Trim(),
                    SoftwareId = request.SoftwareId,
                    IsDefault = request.IsDefault,
                    CreateDate = DateTime.Now.ConvertToTimestamp(),
                    UpdateDate = DateTime.Now.ConvertToTimestamp(),
                }, cancellationToken);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPut("Update")]
        public async Task<ActionResult<BaseResponse>> UpdateAsync([FromBody] UpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            { 
                if (string.IsNullOrEmpty(request.Name) ||
                    request.RoleId == default)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.FillRequiredParameter));
                }

                var checkRoleExist = await CheckRoleExistAsync(request.RoleId, cancellationToken);
                if (checkRoleExist != null)
                {
                    return BadRequest(checkRoleExist);
                }

                var validateResult = await ValidateForUpdateRoleAsync(request, cancellationToken);
                if (validateResult != null)
                {
                    return BadRequest(validateResult);
                }

                var role = await UnitOfWork.Roles.FindAsync(s =>
                    s.RoleId == request.RoleId, cancellationToken);
                role.Name = request.Name;
                role.IsDefault = request.IsDefault;
                role.UpdateDate = DateTime.Now.ConvertToTimestamp();
                UnitOfWork.Roles.Update(role);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpDelete("Delete/{roleId}")]
        public async Task<ActionResult<BaseResponse>> DeleteAsync(Guid roleId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var checkRoleExist = await CheckRoleExistAsync(roleId, cancellationToken);
                if (checkRoleExist != null)
                {
                    return BadRequest(checkRoleExist);
                }

                UnitOfWork.Roles.Delete(s => s.RoleId == roleId);
                UnitOfWork.RolePermissions.DeleteRange(s => s.RoleId == roleId);
                UnitOfWork.UserRoles.DeleteRange(s => s.RoleId == roleId);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        #region Methods
        [NonAction]
        private async Task<BaseResponse> ValidateForCreateRoleAsync(CreateRequest request,
          CancellationToken cancellationToken = default)
        {

            if (await UnitOfWork.Roles.AnyAsync(r => r.SoftwareId == request.SoftwareId &&
                r.Name == request.Name, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.RoleNameAlreadyExist);
            }
            if (request.IsDefault && await UnitOfWork.Roles.AnyAsync(r =>
                r.SoftwareId == request.SoftwareId && r.IsDefault, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.OneDefaultRoleOnly);
            }
            return null;
        }

        [NonAction]
        private async Task<BaseResponse> ValidateForUpdateRoleAsync(UpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            var role = await UnitOfWork.Roles.FindAsync(r => r.RoleId == request.RoleId, cancellationToken);
            if (await UnitOfWork.Roles.AnyAsync(r => r.RoleId != request.RoleId &&
                r.Name == request.Name && r.SoftwareId == role.SoftwareId, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.RoleNameAlreadyExist);
            }
            if (request.IsDefault && await UnitOfWork.Roles.AnyAsync(r =>
                r.SoftwareId == role.SoftwareId && r.RoleId != request.RoleId && r.IsDefault, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.OneDefaultRoleOnly);
            }
            return null;
        }
        #endregion
    }
}
