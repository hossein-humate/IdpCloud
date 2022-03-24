using AutoMapper;
using IdpCloud.REST.Infrastructure.Controller;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IdpCloud.REST.Areas.Identity.Structure
{
    [Area("Identity")]
    [ApiExplorerSettings(GroupName = "Identity")]
    public class IdentityBaseController : ApiBaseController
    {
        public IdentityBaseController(IUnitOfWork unitOfWork, IConfiguration configuration,
            IMapper mapper, ICurrentUserSessionService currentUserSession) :
            base(unitOfWork, configuration, mapper, currentUserSession)
        {
        }
    }
}
