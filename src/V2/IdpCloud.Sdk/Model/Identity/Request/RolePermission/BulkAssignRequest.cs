using System;
using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.Identity.Request.RolePermission
{
    public class BulkAssignRequest
    {
        public BulkAssignRequest()
        {
            AssignItems = new HashSet<AssignItem>();
        }

        public Guid RoleId { get; set; }
        public ICollection<AssignItem> AssignItems { get; set; }
    }

    public class AssignItem
    {
        public Guid PermissionId { get; set; }
        public bool Assign { get; set; }
    }
}
