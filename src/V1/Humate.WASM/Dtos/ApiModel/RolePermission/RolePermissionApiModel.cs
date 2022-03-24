using Humate.WASM.Dtos.ApiModel.Permission;
using Humate.WASM.Dtos.ApiModel.Role;
using System;

namespace Humate.WASM.Dtos.ApiModel.RolePermission
{
    public class RolePermissionApiModel
    {
        public Guid RolePermissionId { get; set; }

        public Guid RoleId { get; set; }

        public Guid PermissionId { get; set; }

        public PermissionApiModel Permission { get; set; }

        public RoleApiModel Role { get; set; }
    }
}
