using System;

namespace Humate.Sdk.Model.Identity.Request.Role
{
    public class CreateRequest
    {
        public Guid SoftwareId { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }
    }
}
