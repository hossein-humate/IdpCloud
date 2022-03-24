using System;
using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Request.UserRole
{
    public class CreateRequest
    {
        public IEnumerable<Guid?> UserIds { get; set; }
        public Guid RoleId { get; set; }
    }
}
