using System;
using System.Collections.Generic;
using System.Text;

namespace IdpCloud.Sdk.Model.Identity.Response.Role
{
    public class GetRoleBySoftwareIdResponse : BaseResponse
    { 
        public IEnumerable<Identity.Role> Roles { get; set; }
    }
}
