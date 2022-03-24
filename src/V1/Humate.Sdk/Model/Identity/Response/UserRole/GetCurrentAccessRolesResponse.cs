using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Response.UserRole
{
    public class GetCurrentAccessRolesResponse : BaseResponse
    {
        public IEnumerable<Identity.Role> AccessRoles { get; set; }
    }
}
