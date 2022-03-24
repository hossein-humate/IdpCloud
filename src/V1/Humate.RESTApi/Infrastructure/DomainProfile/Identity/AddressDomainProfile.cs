using AutoMapper;
using Entity.Identity;
using Humate.Sdk.Model.Identity.Request.Address;

namespace Humate.RESTApi.Infrastructure.DomainProfile.Identity
{
    public class AddressDomainProfile : Profile
    {
        public AddressDomainProfile()
        {
            CreateMap<Address, Sdk.Model.Identity.Address>();
            CreateMap<CreateRequest, Address>();
            CreateMap<UpdateRequest, Address>();
        }
    }
}
