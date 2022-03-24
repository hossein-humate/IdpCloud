using System;
using System.Collections.Generic;
using System.Text;

namespace IdpCloud.Sdk.Model.Identity.Request.Role
{
   public class CreateRoleRequest
    {
        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public string Description { get; set; }

        public Guid  SoftwareId { get; set; }

    }
}
