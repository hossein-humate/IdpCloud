using AutoMapper;
using IdpCloud.DataProvider.Entity.SSO;

namespace IdpCloud.REST.Infrastructure.DomainProfile.SSO
{
    public class UserSessionDomainProfile : Profile
    {
        public UserSessionDomainProfile()
        {
            CreateMap<UserSession, Sdk.Model.SSO.UserSession>();
        }
    }
}
