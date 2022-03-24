using System;
using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.Identity.Request.UserSoftware
{
    public class BulkToggleAssignRequest
    {
        public BulkToggleAssignRequest()
        {
            ToggleAssignItems = new HashSet<ToggleAssignItem>();
        }

        public Guid SoftwareId { get; set; }
        public ICollection<ToggleAssignItem> ToggleAssignItems { get; set; }
    }

    public class ToggleAssignItem
    {
        public Guid UserId { get; set; }
        public bool Assign { get; set; }
    }
}
