using System;
using System.Collections.Generic;

namespace Humate.WASM.Dtos.ApiModel.UserRole.Request
{
    public class CreateRequest
    {
        public IEnumerable<Guid> UserIds { get; set; }

        public Guid RoleId { get; set; }
    }
}
