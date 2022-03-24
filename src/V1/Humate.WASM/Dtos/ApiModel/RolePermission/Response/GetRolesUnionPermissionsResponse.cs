using System.Collections.Generic;
using Humate.WASM.Dtos.ApiModel.Permission;
using Humate.WASM.Dtos.ApiModel.Role;

namespace Humate.WASM.Dtos.ApiModel.RolePermission.Response
{
    public class GetRolesUnionPermissionsResponse : BaseResponse
    {
        public IEnumerable<PermissionApiModel> Permissions { get; set; }
        public IEnumerable<RoleApiModel> Roles { get; set; }
    }
}
