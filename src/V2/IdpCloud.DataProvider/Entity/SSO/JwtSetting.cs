using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IdpCloud.DataProvider.Entity.Identity;

namespace IdpCloud.DataProvider.Entity.SSO
{
    [Table(name: "JwtSettings", Schema = "SSO")]
    public class JwtSetting : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JwtSettingId { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int ExpireMinute { get; set; }

        public bool HasRefresh { get; set; }

        public int RefreshExpireMinute { get; set; }

        public string Secret { get; set; }

        public Guid SoftwareId { get; set; }

        public Software Software { get; set; }
    }
}
