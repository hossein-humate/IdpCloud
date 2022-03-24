using System;
using System.Collections.Generic;

namespace Humate.WASM.Dtos.ApiModel.RolePermission.Request
{
    public class CreateRequest
    {
        public Guid RoleId { get; set; }
        public IEnumerable<Guid> PermissionIds { get; set; }
    }
}
