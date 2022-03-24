using System;

namespace Humate.Sdk.Model.Identity
{
    public class Role
    {
        public Guid RoleId { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public long CreateDate { get; set; }
    }
}
