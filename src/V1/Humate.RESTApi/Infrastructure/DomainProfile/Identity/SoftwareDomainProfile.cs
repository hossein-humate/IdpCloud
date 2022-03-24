using AutoMapper;
using Entity.Identity;

namespace Humate.RESTApi.Infrastructure.DomainProfile.Identity
{
    public class SoftwareDomainProfile : Profile
    {
        public SoftwareDomainProfile()
        {
            CreateMap<Software, Sdk.Model.Identity.Software>()
                .ForMember(dest =>
                    dest.ApiKey, opt => opt.MapFrom(src => ReturnApiKeyValue(src.ApiKey)));
        }

        public static string ReturnApiKeyValue(string value)
        {
            return string.IsNullOrEmpty(value) ? "Not_Created_Yet" : value;
        }
    }
}
