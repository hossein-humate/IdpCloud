using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Identity;

namespace EntityServiceProvider.EntityService.Identity
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface IAddressRepository : IRepository<Address>
    {
    }
}
