using System;
using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Request.Permission
{
    public class UpdateSortOrderRequest
    {
        public IEnumerable<PermissionDto> Permissions { get; set; }
    }

    public class PermissionDto
    {
        public Guid PermissionId { get; set; }

        public short SortOrder { get; set; }

        public Guid ParentId { get; set; }
    }
}
