using System;
using System.Collections.Generic;
using System.Text;

namespace IdpCloud.Sdk.Model.Identity.Response.Role
{
    public class GetAllRoleResponse : BaseResponse
    {
        public IEnumerable<Identity.Role> Roles { get; set; }
    }
}
