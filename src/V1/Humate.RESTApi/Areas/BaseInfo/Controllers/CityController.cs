using AutoMapper;
using EntityServiceProvider;
using Humate.RESTApi.Areas.BaseInfo.Structure;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Humate.Sdk.Model.BaseInfo.Response.City;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Areas.BaseInfo.Controllers
{
    [Route("api/baseinfo/[controller]")]
    public class CityController : BaseInfoBaseController
    {
        public CityController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        {
        }

        [HttpGet("GetByCountryId")]
        public async Task<ActionResult<GetByCountryIdResponse>> GetByCountryIdAsync(short countryId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(new GetByCountryIdResponse
                {
                    Cities = await UnitOfWork.Cities.FindAllAsync<Sdk.Model.BaseInfo
                        .City>(c => c.CountryId == countryId, cancellationToken)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }
    }
}
