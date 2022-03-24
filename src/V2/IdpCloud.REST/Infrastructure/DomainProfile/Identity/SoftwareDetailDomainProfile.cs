using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model.Identity.Request.SoftwareDetail;

namespace IdpCloud.REST.Infrastructure.DomainProfile.Identity
{
    public class SoftwareDetailDomainProfile : Profile
    {
        public SoftwareDetailDomainProfile()
        {
            CreateMap<SoftwareDetail, Sdk.Model.Identity.SoftwareDetail>();
            CreateMap<ModifyRequest, SoftwareDetail>();
        }
    }
}
