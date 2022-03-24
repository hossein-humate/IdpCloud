using System;
using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Request.RolePermission
{
    public class CreateRequest
    {
        public Guid RoleId { get; set; }
        public IEnumerable<Guid?> PermissionIds { get; set; }
    }
}
