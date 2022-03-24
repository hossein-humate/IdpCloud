using AutoMapper;
using IdpCloud.DataProvider.Entity.Identity;
using IdpCloud.Sdk.Model.Identity.Response.RolePermission;

namespace IdpCloud.REST.Infrastructure.DomainProfile.Identity
{
    public class RolePermissionDomainProfile : Profile
    {
        public RolePermissionDomainProfile()
        {
            CreateMap<RolePermission, AssignmentPermission>()
                .ForMember(des => des.PermissionId,
                    op => op.MapFrom(src => src.PermissionId))
                .ForMember(des => des.Name,
                    op => op.MapFrom(src => src.Permission.Name))
                .ForMember(des => des.Action,
                    op => op.MapFrom(src => src.Permission.Action))
                .ForMember(des => des.Scope,
                    op => op.MapFrom(src => src.Permission.Scope))
                .ForMember(des => des.SortOrder,
                    op => op.MapFrom(src => src.Permission.SortOrder))
                .ForMember(des => des.Icon,
                    op => op.MapFrom(src => src.Permission.Icon))
                .ForMember(des => des.ParentId,
                    op => op.MapFrom(src => src.Permission.ParentId))
                .ForMember(des => des.Public,
                    op => op.MapFrom(src => src.Permission.Public))
                .ForMember(des => des.Type,
                    op => op.MapFrom(src => src.Permission.Type))
                .ForMember(des => des.Assigned,
                    op => op.MapFrom(src => src.Role != null))
                .ForMember(des => des.AssignmentDate,
                    op => op.MapFrom(src => src.CreateDate));
        }
    }
}
