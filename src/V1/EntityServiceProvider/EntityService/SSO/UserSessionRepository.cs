using AutoMapper;
using DataProvider.DatabaseContext;
using Entity.SSO;

namespace EntityServiceProvider.EntityService.SSO
{
    public class UserSessionRepository : Repository<UserSession>, IUserSessionRepository
    {
        public UserSessionRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface IUserSessionRepository : IRepository<UserSession>
    {
    }
}
