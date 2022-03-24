using AutoMapper;
using DataProvider.DatabaseContext;
using Entity.Identity;

namespace EntityServiceProvider.EntityService.Identity
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface IPersonRepository : IRepository<Person>
    {
    }
}
