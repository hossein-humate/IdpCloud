using AutoMapper;
using Entity.BaseInfo;
using EntityServiceProvider;
using Humate.RESTApi.Areas.BaseInfo.Structure;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Humate.Sdk.Model.BaseInfo.Request.MasterDetail;
using Humate.Sdk.Model.BaseInfo.Response.MasterDetail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Areas.BaseInfo.Controllers
{
    [Route("api/baseinfo/[controller]")]
    public class MasterDetailController : BaseInfoBaseController
    {
        public MasterDetailController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        {
        }

        [HttpGet("GetAllMaster")]
        public async Task<ActionResult<GetAllMasterResponse>> GetAllMasterAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(new GetAllMasterResponse
                {
                    Masters = await UnitOfWork.MasterDetails.FindAllAsync<Sdk.Model.BaseInfo
                        .MasterDetail>(m => m.MasterId == Guid.Empty && m.SoftwareId == AuthenticatedUser.Software.SoftwareId, cancellationToken)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetAllDetail/{masterId}")]
        public async Task<ActionResult<GetAllDetailResponse>> GetAllDetailAsync(Guid masterId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(new GetAllDetailResponse
                {
                    Details = await UnitOfWork.MasterDetails.FindAllAsync<Sdk.Model.BaseInfo
                        .MasterDetail>(m => m.MasterId == Guid.Empty && m.SoftwareId == AuthenticatedUser.Software.SoftwareId, cancellationToken)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetById/{masterDetailId}")]
        public async Task<ActionResult<GetByIdResponse>> GetByIdAsync(Guid masterDetailId,
            CancellationToken cancellationToken)
        {
            try
            {
                return Ok(new GetByIdResponse
                {
                    MasterDetail = await UnitOfWork.MasterDetails.FindAsync<Sdk.Model.BaseInfo
                        .MasterDetail>(m => m.MasterDetailId == masterDetailId, cancellationToken)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPost("CreateMaster")]
        public async Task<ActionResult<BaseResponse>> CreateMasterAsync([FromBody] CreateMasterRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await UnitOfWork.MasterDetails.AddAsync(new MasterDetail
                {
                    Software = AuthenticatedUser.Software,
                    Name = request.Name,
                    Order = request.Order,
                    Parameter = request.Parameter,
                    MasterId = Guid.Empty
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


        [HttpPut("UpdateMasterDetail")]
        public async Task<ActionResult<BaseResponse>> UpdateMasterDetailAsync([FromBody] UpdateMasterDetailRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var validateResult = await CheckMasterDetailExistAsync(request.MasterDetailId, cancellationToken);
                if (validateResult != null)
                {
                    return BadRequest(validateResult);
                }
                validateResult = await ValidateUpdateMasterDetailAsync(request, cancellationToken);
                if (validateResult != null)
                {
                    return BadRequest(validateResult);
                }

                var master = await UnitOfWork.MasterDetails.FindAsync(m =>
                    m.MasterDetailId == request.MasterDetailId, cancellationToken);
                master.Name = request.Name;
                master.Order = request.Order;
                master.Parameter = request.Parameter;
                UnitOfWork.MasterDetails.Update(master);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpPost("CreateDetail")]
        public async Task<ActionResult<BaseResponse>> CreateDetailAsync([FromBody] CreateDetailRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var validateResult = await CheckMasterDetailExistAsync(request.MasterId, cancellationToken);
                if (validateResult != null)
                {
                    return BadRequest(validateResult);
                }

                await UnitOfWork.MasterDetails.AddAsync(new MasterDetail
                {
                    Software = AuthenticatedUser.Software,
                    Name = request.Name,
                    Order = request.Order,
                    Parameter = request.Parameter,
                    MasterId = request.MasterId
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

        [HttpDelete("Delete/{masterDetailId}")]
        public async Task<ActionResult<BaseResponse>> DeleteAsync(Guid masterDetailId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var checkMasterDetailId = await CheckMasterDetailExistAsync(masterDetailId, cancellationToken);
                if (checkMasterDetailId != null)
                {
                    return BadRequest(checkMasterDetailId);
                }

                UnitOfWork.MasterDetails.Delete(s => s.MasterDetailId == masterDetailId);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [NonAction]
        public async Task<BaseResponse> ValidateUpdateMasterDetailAsync(UpdateMasterDetailRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!await UnitOfWork.MasterDetails.AnyAsync(m => m.MasterDetailId == request.MasterDetailId &&
                m.SoftwareId == AuthenticatedUser.Software.SoftwareId, cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.InvalidMasterDetailId);
            }
            if (await UnitOfWork.MasterDetails.AnyAsync(m => m.MasterDetailId != request.MasterDetailId &&
               m.SoftwareId == AuthenticatedUser.Software.SoftwareId &&
               string.Equals(m.Parameter, request.Parameter, StringComparison.Ordinal), cancellationToken))
            {
                return BaseResponseCollection.GetBaseResponse(RequestResult.ParameterAlreadyExist);

            }
            return null;
        }
    }
}
