using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Identity;

namespace EntityServiceProvider.EntityService.Identity
{
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext,mapper)
        {
        }
    }

    public interface IUserRoleRepository : IRepository<UserRole>
    {
    }
}