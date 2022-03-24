using AutoMapper;
using Entity.Identity;

namespace Humate.RESTApi.Infrastructure.DomainProfile.Identity
{
    public class PersonDomainProfile : Profile
    {
        public PersonDomainProfile()
        {
            CreateMap<Person, Sdk.Model.Identity.Person>();
            CreateMap<Sdk.Model.Identity.Person, Person>();
        }
    }
}
