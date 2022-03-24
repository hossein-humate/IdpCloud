using AutoMapper;
using Entity.Identity;

namespace Humate.RESTApi.Infrastructure.DomainProfile.Identity
{
    public class UserDomainProfile : Profile
    {
        public UserDomainProfile()
        {
            CreateMap<User, Sdk.Model.Identity.User>();
        }
    }
}
