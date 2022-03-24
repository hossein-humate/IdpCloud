using System;

namespace IdpCloud.Sdk.Model.Identity.Request.UserSoftware
{
    public class ToggleAssignRequest
    {
        public bool Assign { get; set; }
        public Guid UserId { get; set; }
        public Guid SoftwareId { get; set; }
    }
}
