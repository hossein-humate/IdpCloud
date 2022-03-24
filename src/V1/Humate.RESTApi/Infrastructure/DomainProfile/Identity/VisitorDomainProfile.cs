using AutoMapper;
using Entity.Identity;

namespace Humate.RESTApi.Infrastructure.DomainProfile.Identity
{
    public class VisitorDomainProfile : Profile
    {
        public VisitorDomainProfile()
        {
            CreateMap<Visitor, Sdk.Model.Identity.Visitor>();
        }
    }
}
