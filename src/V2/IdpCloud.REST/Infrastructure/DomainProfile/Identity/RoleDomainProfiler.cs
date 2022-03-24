using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;

namespace IdpCloud.REST.Infrastructure.DomainProfile.Identity
{
    public class RoleDomainProfiler : Profile
    {
        public RoleDomainProfiler()
        {
            CreateMap<Role, Sdk.Model.Identity.Role>();

        }
    }
}
