using AutoMapper;
using Entity.BaseInfo;

namespace Humate.RESTApi.Infrastructure.DomainProfile.BaseInfo
{
    public class MasterDetailDomainProfile : Profile
    {
        public MasterDetailDomainProfile()
        {
            CreateMap<MasterDetail, Sdk.Model.BaseInfo.MasterDetail>();
        }
    }
}
