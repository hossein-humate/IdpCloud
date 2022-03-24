using System;

namespace Humate.Sdk.Model.Identity.Request.User
{
    public class AssignToSoftwareRequest
    {
        public Guid UserId { get; set; }
        public Guid SoftwareId { get; set; }
    }
}
