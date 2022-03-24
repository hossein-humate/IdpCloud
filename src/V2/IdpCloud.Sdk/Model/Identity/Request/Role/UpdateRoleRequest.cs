using System;

namespace IdpCloud.Sdk.Model.Identity.Request.Role
{
    public class UpdateRoleRequest
    {
        public Guid RoleId { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public string Description { get; set; }

        public Guid SoftwareId { get; set; }
    }
}
