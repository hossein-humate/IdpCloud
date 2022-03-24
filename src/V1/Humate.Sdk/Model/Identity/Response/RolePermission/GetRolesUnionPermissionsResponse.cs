using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Response.RolePermission
{
    public class GetRolesUnionPermissionsResponse : BaseResponse
    {
        public IEnumerable<Identity.Permission> Permissions { get; set; }
        public IEnumerable<Identity.Role> Roles { get; set; }
    }
}
