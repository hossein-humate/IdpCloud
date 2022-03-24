using AutoMapper;
using Entity.BaseInfo;

namespace Humate.RESTApi.Infrastructure.DomainProfile.BaseInfo
{
    public class LanguageDomainProfile : Profile
    {
        public LanguageDomainProfile()
        {
            CreateMap<Language, Sdk.Model.BaseInfo.Language>();
        }
    }
}
