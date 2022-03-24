using AutoMapper;
using Entity.Identity;

namespace Humate.RESTApi.Infrastructure.DomainProfile.Identity
{
    public class RoleDomainProfile : Profile
    {
        public RoleDomainProfile()
        {
            CreateMap<Role, Sdk.Model.Identity.Role>();
        }
    }
}
