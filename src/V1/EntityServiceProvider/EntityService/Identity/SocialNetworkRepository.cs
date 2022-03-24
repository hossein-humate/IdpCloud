using AutoMapper;
using DataProvider.DatabaseContext;
using  Entity.Identity;

namespace EntityServiceProvider.EntityService.Identity
{
    public class SocialNetworkRepository : Repository<SocialNetwork>, ISocialNetworkRepository
    {
        public SocialNetworkRepository(EfCoreContext databaseContext, IMapper mapper)
            : base(databaseContext,mapper)
        {
        }
    }

    public interface ISocialNetworkRepository : IRepository<SocialNetwork>
    {
    }
}