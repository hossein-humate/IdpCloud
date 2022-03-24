using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.BaseInfo;

namespace EntityServiceProvider.EntityService.BaseInfo
{
    public class MasterDetailRepository : Repository<MasterDetail>, IMasterDetailRepository
    {
        public MasterDetailRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface IMasterDetailRepository : IRepository<MasterDetail>
    {
    }
}
