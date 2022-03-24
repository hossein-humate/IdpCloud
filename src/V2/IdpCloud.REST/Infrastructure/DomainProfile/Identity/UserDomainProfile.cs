using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model.Identity.Request.User;

namespace IdpCloud.REST.Infrastructure.DomainProfile.Identity
{
    public class UserDomainProfile : Profile
    {
        public UserDomainProfile()
        {
            CreateMap<User, Sdk.Model.Identity.User>().ReverseMap();
            CreateMap<User, Sdk.Model.Identity.NewUser>().ReverseMap();

            CreateMap<CreateUserRequest, User>();
            CreateMap<UpdateUserRequest, User>();
        }
    }
}