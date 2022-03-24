using AutoMapper;
using IdpCloud.ServiceProvider;
using IdpCloud.ServiceProvider.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IdpCloud.REST.Infrastructure.Controller
{
    /// <summary>
    /// The <see cref="ApiBaseController"/> class inherited from <see cref="ControllerBase"/> class, that is provide general and global services, methods, parameters  
    /// </summary>
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        /// <summary>
        /// Configuration interface provide access to the appsetting.json data depend on Environment(Production,Develpment,...) that is already hosted the WebApplication 
        /// </summary>
        protected readonly IConfiguration Configuration;

        /// <summary>
        /// Provide access to the Automapper lib functionalities 
        /// </summary>
        protected readonly IMapper Mapper;

        /// <summary>
        /// This Parameter load the current UserSession User data depend on claims those are extracted form Request Auth-Header it will contain
        /// <see cref="Entity.Identity.User"/>,
        ///  <see cref="Entity.Identity.Software"/>, <see cref="Entity.SSO.UserSession"/> and etc.
        /// </summary>
        protected readonly ICurrentUserSessionService CurrentUserSession;

        /// <summary>
        /// Provide access to all Database Contexts(Like the <see cref="DataProvider.DatabaseContext.EfCoreContext"/>) and Repositories that has been registered inside it
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; }

        public ApiBaseController()
        {

        }

        public ApiBaseController(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper,
            ICurrentUserSessionService currentUserSession)
        {
            UnitOfWork = unitOfWork;
            Configuration = configuration;
            Mapper = mapper;
            CurrentUserSession = currentUserSession;
        }
    }
}
