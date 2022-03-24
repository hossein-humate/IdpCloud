using AutoMapper;
using EntityServiceProvider;
using Humate.RESTApi.Infrastructure.Authentication.Model;
using Humate.RESTApi.Infrastructure.Controller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Humate.RESTApi.Areas.SSO.Structure
{
    [Area("SSO")]
    [ApiExplorerSettings(GroupName = "SSO")]
    public class SsoBaseController : ApiBaseController
    {
        public SsoBaseController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, IAuthenticatedUser authenticatedUser) :
            base(unitOfWork, configuration, mapper, authenticatedUser)
        {
        }
    }
}
