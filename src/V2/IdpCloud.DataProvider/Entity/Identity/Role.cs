using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdpCloud.DataProvider.Entity.Identity
{
    [Table(name: "Roles", Schema = "Identity")]
    public class Role : BaseEntity
    {
        public Role()
        {
            RoleId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid RoleId { get; set; }

        public Guid SoftwareId { get; set; }

        public Software Software { get; set; }

        public string Name { get; set; }

        [DefaultValue(false)]
        public bool IsDefault { get; set; }

        public string Description { get; set; }

        public List<RolePermission> RolePermissions { get; set; }

        public List<UserRole> UserRoles { get; set; }
    }
}
