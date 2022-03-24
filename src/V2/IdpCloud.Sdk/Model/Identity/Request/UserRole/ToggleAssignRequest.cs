using System;
using System.Collections.Generic;
using System.Text;

namespace IdpCloud.Sdk.Model.Identity.Request.UserRole
{
    public class ToggleAssignRequest
    {
        public bool Assign { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
