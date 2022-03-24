using AutoMapper;
using EntityServiceProvider;
using Humate.RESTApi.Areas.Identity.Structure;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Humate.Sdk.Model.Identity;
using Humate.Sdk.Model.Identity.Request.Address;
using Humate.Sdk.Model.Identity.Response.Address;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Areas.Identity.Controllers
{
    [Route("api/identity/[controller]")]
    public class AddressController : IdentityBaseController
    {
        public AddressController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        { }

        [HttpGet("GetByUserId")]
        public async Task<ActionResult<GetByUserIdResponse>> GetByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var checkRoleExist = await CheckUserExistAsync(userId, cancellationToken);
                if (checkRoleExist != null)
                {
                    return BadRequest(checkRoleExist);
                }

                return Ok(new GetByUserIdResponse
                {
                    Addresses = Mapper.Map<IEnumerable<Address>>(await UnitOfWork.Addresses.FindAllAsync(a =>
                        a.UserId == userId, cancellationToken, a => a.City, a => a.Country))
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }

        [HttpPost("Create")]
        public async Task<ActionResult<BaseResponse>> CreateAsync([FromBody] CreateRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var checkExist = await CheckUserExistAsync(request.UserId, cancellationToken);
                if (checkExist != null)
                {
                    return BadRequest(checkExist);
                }

                checkExist = await CheckCountryExistAsync(request.CountryId, cancellationToken);
                if (checkExist != null)
                {
                    return BadRequest(checkExist);
                }

                var checkUserExist = await CheckCityExistAsync(request.CityId, cancellationToken);
                if (checkUserExist != null)
                {
                    return BadRequest(checkUserExist);
                }

                await UnitOfWork.Addresses.AddAsync(request, cancellationToken);
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
        public async Task<ActionResult<BaseResponse>> UpdateAsync([FromBody] UpdateRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var checkExist = await CheckAddressExistAsync(request.AddressId, cancellationToken);
                if (checkExist != null)
                {
                    return BadRequest(checkExist);
                }

                checkExist = await CheckCountryExistAsync(request.CountryId, cancellationToken);
                if (checkExist != null)
                {
                    return BadRequest(checkExist);
                }

                var checkUserExist = await CheckCityExistAsync(request.CityId, cancellationToken);
                if (checkUserExist != null)
                {
                    return BadRequest(checkUserExist);
                }

                await UnitOfWork.Addresses.UpdateAsync(a => a.AddressId == request.AddressId,
                    request, cancellationToken);
                await UnitOfWork.CompleteAsync(cancellationToken);

                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<BaseResponse>> DeleteAsync(Guid addressId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var checkExist = await CheckAddressExistAsync(addressId, cancellationToken);
                if (checkExist != null)
                {
                    return BadRequest(checkExist);
                }

                UnitOfWork.Addresses.Delete(s => s.AddressId == addressId);
                await UnitOfWork.CompleteAsync(cancellationToken);
                return Ok(new BaseResponse());
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection
                    .GetBaseResponse(RequestResult.UnHandledException));
            }
        }
    }
}
