using System;

namespace Humate.WASM.Dtos.ApiModel.Permission.Request
{
    public class UpdateRequest
    {
        public Guid PermissionId { get; set; }

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
