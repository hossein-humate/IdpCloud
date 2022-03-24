using AutoMapper;
using Entity.Identity;

namespace Humate.RESTApi.Infrastructure.DomainProfile.Identity
{
    public class RolePermissionDomainProfile : Profile
    {
        public RolePermissionDomainProfile()
        {
            CreateMap<RolePermission, Sdk.Model.Identity.RolePermission>();
            CreateMap<RolePermission, Sdk.Model.Identity.Permission>()
                .ForMember(des => des.Name, op => op.MapFrom(src => src.Permission.Name))
                .ForMember(des => des.Action, op => op.MapFrom(src => src.Permission.Action))
                .ForMember(des => des.Scope, op => op.MapFrom(src => src.Permission.Scope))
                .ForMember(des => des.PermissionId, op => op.MapFrom(src => src.PermissionId))
                .ForMember(des => des.Icon, op => op.MapFrom(src => src.Permission.Icon))
                .ForMember(des => des.ParentId, op => op.MapFrom(src => src.Permission.ParentId))
                .ForMember(des => des.Public, op => op.MapFrom(src => src.Permission.Public))
                .ForMember(des => des.SortOrder, op => op.MapFrom(src => src.Permission.SortOrder))
                .ForMember(des => des.Type, op => op.MapFrom(src => src.Permission.Type))
                .ForMember(des => des.Childrens, op => op.MapFrom(src => src.Permission.Childrens));
        }
    }
}
