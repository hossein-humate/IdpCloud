using AutoMapper;
using IdpCloud.DataProvider.Entity.SSO;
using IdpCloud.Sdk.Model.SSO.Request.JwtSetting;

namespace IdpCloud.REST.Infrastructure.DomainProfile.SSO
{
    public class JwtSettingDomainProfile : Profile
    {
        public JwtSettingDomainProfile()
        {
            CreateMap<JwtSetting, Sdk.Model.SSO.JwtSetting>();
            CreateMap<ModifyRequest, JwtSetting>();
        }
    }
}
