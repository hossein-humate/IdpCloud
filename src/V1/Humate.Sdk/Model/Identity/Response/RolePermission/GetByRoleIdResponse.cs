using System;
using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Response.RolePermission
{
    public class GetByRoleIdResponse : BaseResponse
    {
        public IEnumerable<Identity.Permission> Permissions { get; set; }
        public Guid RoleId { get; set; }
    }
}
