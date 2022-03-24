using AutoMapper;
using Entity.BaseInfo;

namespace Humate.RESTApi.Infrastructure.DomainProfile.BaseInfo
{
    public class CityDomainProfile : Profile
    {
        public CityDomainProfile()
        {
            CreateMap<City, Sdk.Model.BaseInfo.City>();
        }
    }
}
