using AutoMapper;
using IdpCloud.DataProvider.Entity.BaseInfo;

namespace IdpCloud.REST.Infrastructure.DomainProfile.BaseInfo
{
    public class CountryDomainProfile : Profile
    {
        public CountryDomainProfile()
        {
            CreateMap<Country, Sdk.Model.BaseInfo.Country>();
        }
    }
}
