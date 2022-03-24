using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdpCloud.DataProvider.Entity.Identity
{
    [Table("UserPermission", Schema = "Identity")]
    public class UserPermission : BaseEntity
    {
        public UserPermission()
        {
            UserPermissionId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserPermissionId { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public Guid PermissionId { get; set; }

        public Permission Permission { get; set; }
    }
}