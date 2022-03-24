using System;

namespace Humate.Sdk.Model.Identity
{
    public class UserRole
    {
        public Guid UserRoleId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
