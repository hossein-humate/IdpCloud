using System;

namespace Humate.Sdk.Model.Identity.Request.Role
{
    public class UpdateRequest
    {
        public Guid RoleId { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}
