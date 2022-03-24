using AutoMapper;
using IdpCloud.DataProvider.Entity.BaseInfo;

namespace IdpCloud.REST.Infrastructure.DomainProfile.BaseInfo
{
    public class LanguageDomainProfile : Profile
    {
        public LanguageDomainProfile()
        {
            CreateMap<Language, Sdk.Model.BaseInfo.Language>();
        }
    }
}
