using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Humate.WASM.Shared.Component.Icon;
using Humate.WASM.Shared.Component.TreeView;

namespace Humate.WASM.Dtos.ApiModel.Permission
{
    public class PermissionDomainProfile : Profile
    {
        public PermissionDomainProfile()
        {
            CreateMap<PermissionApiModel, NodeItem>()
                .ForMember(des => des.Id, map => map.MapFrom(src => src.PermissionId))
                .ForMember(des => des.ParentId, map => map.MapFrom(src => src.ParentId))
                .ForMember(des => des.Icon, map => map.MapFrom(src =>
                        MapIconFromType(src.Type)))
                .ForMember(des => des.Text, map => map.MapFrom(src => src.Name));
        }

        private static SvgName MapIconFromType(PermissionType type) =>
            type switch
            {
                PermissionType.Link => SvgName.Link,
                PermissionType.Page => SvgName.Document,
                PermissionType.ActionResult => SvgName.Collection,
                PermissionType.Component => SvgName.CubeTransparent,
                _ => SvgName.Archive
            };
    }
}
