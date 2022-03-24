using AutoMapper;
using Entity.BaseInfo;

namespace Humate.RESTApi.Infrastructure.DomainProfile.BaseInfo
{
    public class CurrencyDomainProfile : Profile
    {
        public CurrencyDomainProfile()
        {
            CreateMap<Currency, Sdk.Model.BaseInfo.Currency>();
        }
    }
}
