using AutoMapper;
using EntityServiceProvider;
using General;
using Humate.RESTApi.Areas.Identity.Structure;
using Humate.RESTApi.Infrastructure.Authentication;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.Sdk.Model;
using Humate.Sdk.Model.Identity.Response.Visitor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Humate.RESTApi.Areas.Identity.Controllers
{
    [Route("api/identity/[controller]")]
    public class VisitorController : IdentityBaseController
    {
        public VisitorController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        { }

        [HttpGet("GetAll")]
        public async Task<ActionResult<GetAllResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return Ok(new GetAllResponse
                {
                    Visitors = await UnitOfWork.Visitors.FindAllAsync<Sdk.Model.Identity
                        .Visitor>(v => true, cancellationToken)
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetDailyLastWeek")]
        public async Task<ActionResult<GetDailyLastWeekResponse>> GetDailyLastWeekAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var lastWeekDate = DateTime.Now.AddDays(-7).ConvertToTimestamp();
                var visitors = (await UnitOfWork.Visitors.FindAllAsync(v =>
                    lastWeekDate < v.CreateDate, cancellationToken)).Select(v => new
                    {
                        v.VisitorId,
                        CreationDate = v.CreateDate
                    }).ToList();
                var lastWeek = visitors.GroupBy(v => v.CreationDate).Select(v => new DailyVisit
                {
                    Count = visitors.Count(x => x.CreationDate == v.Key),
                    Date = v.Key.UnixTimeStampToDateTime().Date,
                    DayOfWeek = v.Key.UnixTimeStampToDateTime().DayOfWeek
                }).OrderBy(l => l.Date);
                return Ok(new GetDailyLastWeekResponse
                {
                    LastWeek = lastWeek
                });
            }
            catch (Exception)
            {
                return BadRequest(BaseResponseCollection.GetBaseResponse(
                    RequestResult.UnHandledException));
            }
        }

        [HttpGet("GetById/{visitorId}")]
        public async Task<ActionResult<GetByIdResponse>> GetById(Guid visitorId, CancellationToken cancellationToken = default)
        {
            try
            {
                var visitor = await UnitOfWork.Visitors.FindAsync<Sdk.Model.Identity.Visitor>(v =>
                        v.VisitorId == visitorId, cancellationToken);
                if (visitor == null)
                {
                    return BadRequest(BaseResponseCollection.GetBaseResponse(
                        RequestResult.InvalidDataEntries));
                }
                return Ok(new GetByIdResponse
                {
                    Visitor = visitor
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