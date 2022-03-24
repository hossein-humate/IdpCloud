using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.BaseInfo;

namespace EntityServiceProvider.EntityService.BaseInfo
{
    public class CountryRepository : Repository<Country>, ICountryRepository
    {
        public CountryRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface ICountryRepository : IRepository<Country>
    {
    }
}
