using System.Collections.Generic;

namespace Humate.Sdk.Model.Identity.Response.Permission
{
    public class GetOnRoleStateAndTreeFormatResponse : BaseResponse
    {
        public IEnumerable<PermissionTree> Permissions { get; set; }
    }

    public class PermissionTree 
    {
        public string PermissionId { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public TreeState State { get; set; }
    }

    public class TreeState
    {
        public bool Opened { get; set; }
        public bool Disabled { get; set; }
        public bool Selected { get; set; }
        public bool Checked { get; set; }
    }
}
