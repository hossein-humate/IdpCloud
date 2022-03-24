using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Payment;

namespace EntityServiceProvider.EntityService.Payment
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface IInvoiceRepository : IRepository<Invoice>
    {
    }
}
