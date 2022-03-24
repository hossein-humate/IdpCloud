using AutoMapper;
using Entity.SSO;

namespace Humate.RESTApi.Infrastructure.DomainProfile.SSO
{
    public class UserSessionDomainProfile : Profile
    {
        public UserSessionDomainProfile()
        {
            CreateMap<UserSession, Sdk.Model.SSO.UserSession>();
        }
    }
}
