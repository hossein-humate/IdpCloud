using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Identity;

namespace EntityServiceProvider.EntityService.Identity
{
    public class VisitorRepository : Repository<Visitor>, IVisitorRepository
    {
        public VisitorRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext,mapper)
        {
        }
    }
    public interface IVisitorRepository : IRepository<Visitor>
    {
    }
}
