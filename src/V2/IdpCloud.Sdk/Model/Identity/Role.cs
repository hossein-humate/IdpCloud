using System;

namespace IdpCloud.Sdk.Model.Identity
{
    public class Role
    {
        public Guid RoleId { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public string Description { get; set; }

        public Guid SoftwareId { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
