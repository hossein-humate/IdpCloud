using AutoMapper;
using Entity.Identity;

namespace Humate.RESTApi.Infrastructure.DomainProfile.Identity
{
    public class PermissionDomainProfile : Profile
    {
        public PermissionDomainProfile()
        {
            CreateMap<Permission, Sdk.Model.Identity.Permission>();
        }
    }
}
