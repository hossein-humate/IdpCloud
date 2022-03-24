using AutoMapper;
using EntityServiceProvider;
using Humate.RESTApi.Areas.BaseInfo.Structure;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Humate.Sdk.Model.BaseInfo.Response.Country;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Areas.BaseInfo.Controllers
{
    [Route("api/baseinfo/[controller]")]
    public class CountryController : BaseInfoBaseController
    {
        public CountryController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        {
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<GetAllResponse>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(new GetAllResponse
                {
                    Countries = await UnitOfWork.Countries.FindAllAsync<Sdk.Model.BaseInfo
                        .Country>(c => c.IsActive.Value, cancellationToken)
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
