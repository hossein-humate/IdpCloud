using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;

namespace IdpCloud.REST.Infrastructure.DomainProfile.Identity
{
    public class SoftwareDomainProfile : Profile
    {
        public SoftwareDomainProfile()
        {
            CreateMap<Software, Sdk.Model.Identity.Software>();
            CreateMap<Software, Sdk.Model.SSO.Response.UserSoftware.SoftwareDto>()
                .ForMember(des => des.UrlOrPath, op => op.MapFrom(src => src.SoftwareDetail.ProductionPath));
        }
    }
}
