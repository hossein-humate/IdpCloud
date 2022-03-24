using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Identity
{
    [Table(name: "Permissions", Schema = "Identity")]
    public class Permission : BaseEntity
    {
        public Permission()
        {
            PermissionId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid PermissionId { get; set; }

        ~Permission() { Dispose(true); }

        public Guid SoftwareId { get; set; }

        public Software Software { get; set; }

        public string Name { get; set; }

        public string Action { get; set; }

        public string Scope { get; set; }

        public Guid ParentId { get; set; }

        public PermissionType Type { get; set; }

        public short SortOrder { get; set; }

        [DefaultValue(false)]
        public bool Public { get; set; }

        public string Icon { get; set; }

        public Permission Parent { get; set; }

        public List<Permission> Childrens { get; set; }

        public List<RolePermission> RolePermissions { get; set; }

        public List<UserPermission> UserPermissions { get; set; }
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
