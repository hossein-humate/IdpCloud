using AutoMapper;
using Entity.Identity;
using Humate.Sdk.Model.Identity.Response.UserRole;

namespace Humate.RESTApi.Infrastructure.DomainProfile.Identity
{
    public class UserRoleDomainProfile : Profile
    {
        public UserRoleDomainProfile()
        {
            CreateMap<UserRole, Sdk.Model.Identity.UserRole>();
            CreateMap<UserRole, GetByRoleId>()
                .ForMember(des => des.UserRoleId, op => op.MapFrom(src => src.UserRoleId))
                .ForMember(des => des.Name, op => op.MapFrom(src => src.Role.Name))
                .ForMember(des => des.IsDefault, op => op.MapFrom(src => src.Role.IsDefault))
                .ForMember(des => des.RoleId, op => op.MapFrom(src => src.RoleId))
                .ForMember(des => des.Firstname, op => op.MapFrom(src => src.User.Person.Firstname))
                .ForMember(des => des.Lastname, op => op.MapFrom(src => src.User.Person.Lastname))
                .ForMember(des => des.Username, op => op.MapFrom(src => src.User.Username))
                .ForMember(des => des.UserId, op => op.MapFrom(src => src.UserId));
            CreateMap<UserRole, Sdk.Model.Identity.Role>()
                .ForMember(des => des.Name, op => op.MapFrom(src => src.Role.Name))
                .ForMember(des => des.CreateDate, op => op.MapFrom(src => src.Role.CreateDate))
                .ForMember(des => des.IsDefault, op => op.MapFrom(src => src.Role.IsDefault))
                .ForMember(des => des.RoleId, op => op.MapFrom(src => src.RoleId));
        }
    }
}
