using System;
using System.Collections.Generic;
using Humate.WASM.Dtos.ApiModel.Permission;

namespace Humate.WASM.Dtos.ApiModel.RolePermission.Response
{
    public class GetByRoleIdResponse:BaseResponse
    {
        public IEnumerable<PermissionApiModel> Permissions { get; set; }
        public Guid RoleId { get; set; }
    }
}
