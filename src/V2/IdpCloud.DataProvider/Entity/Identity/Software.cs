using IdpCloud.DataProvider.Entity.SSO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdpCloud.DataProvider.Entity.Identity
{
    [Table("Softwares", Schema = "Identity")]
    public class Software : BaseEntity
    {
        public Software()
        {
            SoftwareId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid SoftwareId { get; set; }

        public string Name { get; set; }

        public string Brand { get; set; }

        public string BusinessDescription { get; set; }

        public string LogoImage { get; set; }

        public string ApiKey { get; set; }

        public DateTime KeyExpire { get; set; }

        public Guid? OwnerUserId { get; set; }

        public SoftwareStatus Status { get; set; }

        public JwtSetting JwtSetting { get; set; }

        public SoftwareDetail SoftwareDetail { get; set; }

        public List<UserSoftware> UserSoftwares { get; set; }

        public List<Role> Roles { get; set; }

        public List<Permission> Permissions { get; set; }

        public List<UserSession> UserSessions { get; set; }
    }

    public enum SoftwareStatus
    {
        Active,
        Deactive
    }
}
