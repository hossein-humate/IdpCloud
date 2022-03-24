using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Identity;

namespace EntityServiceProvider.EntityService.Identity
{
    public class UserSoftwareRepository : Repository<UserSoftware>, IUserSoftwareRepository
    {
        public UserSoftwareRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }
    public interface IUserSoftwareRepository : IRepository<UserSoftware>
    {
    }
}
