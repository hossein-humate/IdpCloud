using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Payment;

namespace EntityServiceProvider.EntityService.Payment
{
    public class InvoiceItemRepository : Repository<InvoiceItem>, IInvoiceItemRepository
    {
        public InvoiceItemRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext, mapper)
        {
        }
    }

    public interface IInvoiceItemRepository : IRepository<InvoiceItem>
    {
    }
}
