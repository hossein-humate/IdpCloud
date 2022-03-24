using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Identity
{
    [Table(name: "UserSoftwares", Schema = "Identity")]
    public class UserSoftware : BaseEntity
    {
        public UserSoftware()
        {
            UserSoftwareId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserSoftwareId { get; set; }
        ~UserSoftware() { Dispose(true); }
        public Guid UserId { get; set; }

        public Guid SoftwareId { get; set; }

        public User User { get; set; }

        public Software Software { get; set; }
    }
}
