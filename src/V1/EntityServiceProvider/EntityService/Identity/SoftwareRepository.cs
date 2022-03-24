using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Identity;

namespace EntityServiceProvider.EntityService.Identity
{
    public class SoftwareRepository : Repository<Software>, ISoftwareRepository
    {
        public SoftwareRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }
    public interface ISoftwareRepository : IRepository<Software>
    {
    }
}
