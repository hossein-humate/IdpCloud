using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model.SSO.Request.Organisation;

namespace IdpCloud.REST.Infrastructure.DomainProfile.Identity
{
    /// <summary>
    /// Domain Profile to map Organisation Entity parameters into anathor DTOs
    /// </summary>
    public class OrganisationDomainProfile : Profile
    {
        public OrganisationDomainProfile()
        {
            CreateMap<Organisation, Sdk.Model.Identity.Organisation>().ReverseMap();

            CreateMap<CreateRequest, Organisation>();
            CreateMap<UpdateRequest, Organisation>();
        }
    }
}
