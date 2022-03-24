using System;

namespace Humate.Sdk.Model.Identity.Request.Permission
{
    public class CreateRequest
    {
        public Guid SoftwareId { get; set; }

        public string Name { get; set; }

        public string Action { get; set; }

        public string Scope { get; set; }

        public Guid ParentId { get; set; }

        public PermissionType Type { get; set; }

        public short SortOrder { get; set; }

        public bool Public { get; set; }

        public string Icon { get; set; }
    }
}
