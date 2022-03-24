using System;
using System.Collections.Generic;
using System.Text;

namespace IdpCloud.Sdk.Model.Identity.Request.UserRole
{
    public class BulkToggleAssignRequest
    {
        public BulkToggleAssignRequest()
        {
            ToggleAssignItems = new HashSet<ToggleAssignItem>();
        }

        public Guid RoleId { get; set; }
        public ICollection<ToggleAssignItem> ToggleAssignItems { get; set; }
    }

    public class ToggleAssignItem
    {
        public Guid UserId { get; set; }
        public bool Assign { get; set; }
    }
}
