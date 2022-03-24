using AutoMapper;
using DataProvider.DatabaseContext;
using Entity.BaseInfo;

namespace EntityServiceProvider.EntityService.BaseInfo
{
    public class CityRepository : Repository<City>, ICityRepository
    {
        public CityRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface ICityRepository : IRepository<City>
    {
    }
}
