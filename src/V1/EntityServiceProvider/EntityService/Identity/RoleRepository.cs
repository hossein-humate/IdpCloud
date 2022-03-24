using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Identity;

namespace EntityServiceProvider.EntityService.Identity
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext,mapper)
        {
        }
    }

    public interface IRoleRepository : IRepository<Role>
    {
    }
}
