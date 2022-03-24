using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Identity
{
    [Table(name: "UserRoles", Schema = "Identity")]
    public class UserRole:BaseEntity
    {
        public UserRole()
        {
            UserRoleId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserRoleId { get; set; }

        ~UserRole(){Dispose(true);}

        public Guid UserId { get; set; }
        
        public Guid RoleId { get; set; }

        public User User { get; set; }

        public Role Role { get; set; }
    }
}
