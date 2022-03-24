using System;
using System.Collections.Generic;

namespace Humate.WASM.Dtos.ApiModel.UserRole.Response
{
    public class GetByRoleIdResponse : BaseResponse
    {
        public IEnumerable<GetByRoleId> UserRoles { get; set; }
    }

    public class GetByRoleId
    {
        public Guid UserRoleId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }
}
