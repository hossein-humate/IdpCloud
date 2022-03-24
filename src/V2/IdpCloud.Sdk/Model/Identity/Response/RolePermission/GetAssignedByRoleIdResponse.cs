using System;
using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.Identity.Response.RolePermission
{
    public class GetAssignedByRoleIdResponse : BaseResponse
    {
        public IEnumerable<AssignmentPermission> AssignmentPermissions { get; set; }
    }

    public class AssignmentPermission
    {
        public Guid PermissionId { get; set; }

        public string Name { get; set; }

        public string Action { get; set; }

        public string Scope { get; set; }

        public Guid ParentId { get; set; }

        public PermissionType Type { get; set; }

        public short SortOrder { get; set; }

        public bool Public { get; set; }

        public string Icon { get; set; }

        public bool Assigned { get; set; }

        public DateTime? AssignmentDate { get; set; }
    }
}
