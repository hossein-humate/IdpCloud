using System;
using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.Identity
{
    public class Permission
    {
        public Guid PermissionId { get; set; }

        public string Name { get; set; }

        public string Action { get; set; }
        
        public string Scope { get; set; }
        
        public Guid ParentId { get; set; }

        public PermissionType Type { get; set; }

        public short SortOrder { get; set; }

        public bool Public { get; set; }

        public string Icon { get; set; }

        public virtual IList<Permission> Childrens { get; set; }
    }

    public enum PermissionType : byte
    {
        Software = 0,
        MenuLabel,
        MenuLink,
        Event,
        CommonMethod,
        ActionResult,
        Component,
        UserControl,
        Control,
        Link,
        Service,
        Page,
        PartialView,
        Label
    }
}
