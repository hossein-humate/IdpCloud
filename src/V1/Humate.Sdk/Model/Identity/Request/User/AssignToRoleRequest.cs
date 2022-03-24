using System;

namespace Humate.Sdk.Model.Identity.Request.User
{
    public class AssignToRoleRequest
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
