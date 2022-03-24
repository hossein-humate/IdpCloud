using AutoMapper;
using Entity.BaseInfo;

namespace Humate.RESTApi.Infrastructure.DomainProfile.BaseInfo
{
    public class CountryDomainProfile : Profile
    {
        public CountryDomainProfile()
        {
            CreateMap<Country, Sdk.Model.BaseInfo.Country>();
        }
    }
}
