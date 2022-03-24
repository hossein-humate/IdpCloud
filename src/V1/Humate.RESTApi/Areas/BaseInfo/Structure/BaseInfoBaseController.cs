using AutoMapper;
using EntityServiceProvider;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.RESTApi.Infrastructure.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Humate.RESTApi.Areas.BaseInfo.Structure
{
    [Area("BaseInfo")]
    [ApiExplorerSettings(GroupName = "BaseInfo")]
    public class BaseInfoBaseController : ApiBaseController
    {
        public BaseInfoBaseController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        {
        }
    }
}
