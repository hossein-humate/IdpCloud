﻿using System;

namespace Humate.Sdk.Model.Identity
{
    public class RolePermission
    {
        public Guid RolePermissionId { get; set; }

        public Guid RoleId { get; set; }

        public Guid PermissionId { get; set; }

        public Permission Permission { get; set; }

        public Role Role { get; set; }
    }
}
