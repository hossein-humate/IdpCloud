using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.BaseInfo;

namespace EntityServiceProvider.EntityService.BaseInfo
{
    public class LanguageRepository : Repository<Language>, ILanguageRepository
    {
        public LanguageRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface ILanguageRepository : IRepository<Language>
    {
    }
}
