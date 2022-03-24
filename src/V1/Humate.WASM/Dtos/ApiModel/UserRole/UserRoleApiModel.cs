using System;

namespace Humate.WASM.Dtos.ApiModel.UserRole
{
    public class UserRoleApiModel
    {
        public Guid UserRoleId { get; set; }

        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
    }
}
