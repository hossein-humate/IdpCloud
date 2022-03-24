using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Identity;

namespace EntityServiceProvider.EntityService.Identity
{
    public class UserPermissionRepository : Repository<UserPermission>, IUserPermissionRepository
    {
        public UserPermissionRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface IUserPermissionRepository : IRepository<UserPermission>
    {
    }
}
