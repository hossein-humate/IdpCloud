using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Payment;

namespace EntityServiceProvider.EntityService.Payment
{
    public class InformationRepository : Repository<Information>, IInformationRepository
    {
        public InformationRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface IInformationRepository : IRepository<Information>
    {
    }
}
