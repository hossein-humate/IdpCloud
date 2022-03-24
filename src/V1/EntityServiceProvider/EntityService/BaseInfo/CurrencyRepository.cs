using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.BaseInfo;

namespace EntityServiceProvider.EntityService.BaseInfo
{
    public class CurrencyRepository : Repository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface ICurrencyRepository : IRepository<Currency>
    {
    }
}
