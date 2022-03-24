using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Identity;

namespace EntityServiceProvider.EntityService.Identity
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext,mapper)
        {
        }
    }

    public interface IPermissionRepository : IRepository<Permission>
    {
    }
}