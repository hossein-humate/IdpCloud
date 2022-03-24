using System;

namespace IdpCloud.Sdk.Model.Identity
{
    public class RolePermission
    {
        public Guid RolePermissionId { get; set; }

        public Guid RoleId { get; set; }

        public Guid PermissionId { get; set; }

        public string PermissionName { get; set; }

        public string PermissionAction { get; set; }

        public string PermissionScope { get; set; }

        public Guid PermissionParentId { get; set; }

        public PermissionType PermissionType { get; set; }

        public short PermissionSortOrder { get; set; }

        public bool PermissionPublic { get; set; }

        public string PermissionIcon { get; set; }

        public string RoleName { get; set; }

        public bool RoleIsDefault { get; set; }

        public string RoleDescription { get; set; }

        public Guid SoftwareId { get; set; }
    }
}
