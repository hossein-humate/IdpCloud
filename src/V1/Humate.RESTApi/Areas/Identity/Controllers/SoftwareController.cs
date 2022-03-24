using AutoMapper;
using EntityServiceProvider;
using General;
using Humate.RESTApi.Areas.Identity.Structure;
using Humate.RESTApi.Infrastructure.Authentication;
using Humate.RESTApi.Infrastructure.InternalService.File;
using Humate.Sdk.Model;
using Humate.Sdk.Model.Identity;
using Humate.Sdk.Model.Identity.Request.Software;
using Humate.Sdk.Model.Identity.Response.Software;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Humate.RESTApi.Infrastructure.Authentication.Model;

namespace Humate.RESTApi.Areas.Identity.Controllers
{
    [Route("api/identity/[controller]")]
    public class SoftwareController : IdentityBaseController
    {
        private readonly IFileStorageService _fileStorageService;
        public SoftwareController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser,IFileStorageService fileStorageService) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpGet("GetAll")]
        public ActionResult<GetAllResponse> GetAll()
        {
            try
            {
                if (AuthenticatedUser.AccessSoftwares.Any())
                {
                    return Ok(new GetAllResponse
                    {
                        Softwares = Mapper.Map<IEnumerable<Software>>(AuthenticatedUser.AccessSoftwares)
                    });
                }
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.NotAssignYet));
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetById/{softwareId}")]
        public async Task<ActionResult<GetByIdResponse>> GetByIdAsync(Guid softwareId, CancellationToken cancellationToken = default)
        {
            try
            {
                var software = await UnitOfWork.Softwares.FindAsync<Software>(s =>
                       s.SoftwareId == softwareId, cancellationToken);
                if (software == null)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.InvalidSoftwareId));
                }
                return Ok(new GetByIdResponse
                {
                    Software = software
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<CreateResponse>> CreateAsync([FromBody] CreateRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name) ||
                    string.IsNullOrEmpty(request.BusinessDescription))
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.FillRequiredParameter));
                }

                var software = new Entity.Identity.Software
                {
                    Name = request.Name,
                    Brand = request.Brand,
                    BusinessDescription = request.BusinessDescription,
                    CreateDate = DateTime.Now.ConvertToTimestamp(),
                    UpdateDate = DateTime.Now.ConvertToTimestamp(),
                    OwnerUserId = AuthenticatedUser.User.UserId
                };

                if (!string.IsNullOrEmpty(request.LogoName) && request.LogoContent.Length > 0)
                {
                    var split = request.LogoName.Split(".");
                    var filename = $"{request.Name.Replace(" ","")}_{DateTime.Now.Ticks.ToString().Substring(5, 6)}.{split[^1]}";
                    software.LogoImage = filename;
                    await _fileStorageService.StoreInUserPathAsync(AuthenticatedUser.User.UserId, filename,
                        request.LogoContent,"Project", cancellationToken);
                }
                
                await UnitOfWork.Softwares.AddAsync(software, cancellationToken);
                await UnitOfWork.UserSoftwares.AddAsync(new Entity.Identity.UserSoftware
                {
                    Software = software,
                    UserId = AuthenticatedUser.User.UserId
                }, cancellationToken);

                var role = new Entity.Identity.Role
                {
                    Name = "Default Public Role",
                    SoftwareId = software.SoftwareId,
                    IsDefault = true,
                    CreateDate = DateTime.Now.ConvertToTimestamp(),
                    UpdateDate = DateTime.Now.ConvertToTimestamp()
                };
                await UnitOfWork.Roles.AddAsync(role, cancellationToken);
                await UnitOfWork.UserRoles.AddAsync(new Entity.Identity.UserRole
                {
                    Role = role,
                    UserId = AuthenticatedUser.User.UserId
                }, cancellationToken);

                var permission = await UnitOfWork.Permissions.FindAsync(p =>
                    p.SoftwareId == software.SoftwareId && p.Name == request.Name, cancellationToken);
                if (permission == null)
                {
                    permission = new Entity.Identity.Permission
                    {
                        Name = request.Name,
                        ParentId = Guid.Empty,
                        Action = "/",
                        Public = true,
                        Type = Entity.Identity.PermissionType.Service,
                        SoftwareId = software.SoftwareId
                    };
                    await UnitOfWork.Permissions.AddAsync(permission, cancellationToken);
                    await UnitOfWork.RolePermissions.AddAsync(new Entity.Identity.RolePermission
                    {
                        Role = role,
                        Permission = permission
                    }, cancellationToken);
                }

                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new CreateResponse
                {
                    SoftwareId = software.SoftwareId
                });
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
                var checkSoftwareId = await CheckSoftwareExistAsync(request.SoftwareId, cancellationToken);
                if (checkSoftwareId != null)
                {
                    return BadRequest(checkSoftwareId);
                }

                var software = await UnitOfWork.Softwares.FindAsync(s =>
                    s.SoftwareId == request.SoftwareId, cancellationToken);
                if (software == null)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.InvalidSoftwareId));
                }
                if (string.IsNullOrEmpty(request.Name) ||
                    string.IsNullOrEmpty(request.BusinessDescription))
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.FillRequiredParameter));
                }

                if (!string.IsNullOrEmpty(request.LogoName) && request.LogoContent.Length > 0)
                {
                    var split = request.LogoName.Split(".");
                    var filename = $"{request.Name.Replace(" ", "")}_{DateTime.Now.Ticks.ToString().Substring(5, 6)}.{split[^1]}";
                    software.LogoImage = filename;
                    await _fileStorageService.StoreInUserPathAsync(AuthenticatedUser.User.UserId, filename,
                        request.LogoContent, "Project", cancellationToken);
                }

                software.Name = request.Name;
                software.Brand = request.Brand;
                software.BusinessDescription = request.BusinessDescription;
                software.UpdateDate = DateTime.Now.ConvertToTimestamp();
                UnitOfWork.Softwares.Update(software);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPost("GenerateNewApiKey")]
        public async Task<ActionResult<GenerateNewApiKeyResponse>> GenerateNewApiKeyAsync([FromBody] GenerateNewApiKeyRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var checkSoftwareId = await CheckSoftwareExistAsync(request.SoftwareId, cancellationToken);
                if (checkSoftwareId != null)
                {
                    return BadRequest(checkSoftwareId);
                }

                var software = await UnitOfWork.Softwares.FindAsync(s => s.SoftwareId == request.SoftwareId,
                    cancellationToken);
                software.UpdateDate = DateTime.Now.ConvertToTimestamp();
                software.ApiKey = Cryptography.GenerateSecret(32);
                software.KeyExpire = request.ExpireDate.ConvertToTimestamp(DateTime.Now.AddMonths(1));
                UnitOfWork.Softwares.Update(software);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new GenerateNewApiKeyResponse
                {
                    ApiKey = software.ApiKey,
                    KeyExpireDate = software.KeyExpire.UnixTimeStampToDateTime()
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpDelete("Delete/{softwareId}")]
        public async Task<ActionResult<BaseResponse>> DeleteAsync(Guid softwareId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var checkSoftwareId = await CheckSoftwareExistAsync(softwareId, cancellationToken);
                if (checkSoftwareId != null)
                {
                    return BadRequest(checkSoftwareId);
                }

                UnitOfWork.Softwares.Delete(s => s.SoftwareId == softwareId);
                UnitOfWork.UserSoftwares.DeleteRange(s => s.SoftwareId == softwareId);
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
