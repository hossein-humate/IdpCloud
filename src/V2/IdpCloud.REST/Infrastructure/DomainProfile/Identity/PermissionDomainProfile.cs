using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;

namespace IdpCloud.REST.Infrastructure.DomainProfile.Identity
{
    public class PermissionDomainProfile : Profile
    {
        public PermissionDomainProfile()
        {
            CreateMap<Permission, Sdk.Model.Identity.Permission>();
        }
    }
}
