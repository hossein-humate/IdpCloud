using AutoMapper;
using EntityServiceProvider;
using Humate.RESTApi.Areas.BaseInfo.Structure;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Humate.Sdk.Model.BaseInfo;
using Humate.Sdk.Model.BaseInfo.Response.Language;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Areas.BaseInfo.Controllers
{
    [Route("api/baseinfo/[controller]")]
    public class LanguageController : BaseInfoBaseController
    {
        public LanguageController(IUnitOfWork unitOfWork, IConfiguration configuration,
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
                    Languages = await UnitOfWork.Languages.FindAllAsync<Language>(c =>
                        c.DeleteDate == null, cancellationToken)
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
