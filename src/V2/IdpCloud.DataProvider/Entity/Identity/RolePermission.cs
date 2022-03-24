using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdpCloud.DataProvider.Entity.Identity
{
    [Table(name: "RolePermissions", Schema = "Identity")]
    public class RolePermission : BaseEntity
    {
        public RolePermission()
        {
            RolePermissionId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid RolePermissionId { get; set; }

        public Guid RoleId { get; set; }

        public Guid PermissionId { get; set; }

        public Permission Permission { get; set; }

        public Role Role { get; set; }
    }
}
