using Entity.BaseInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entity.SSO;

namespace Entity.Identity
{
    [Table(name: "Users", Schema = "Identity")]
    public class User : BaseEntity
    {
        public User()
        {
            UserId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid UserId { get; set; }

        ~User() { Dispose(true); }

        [StringLength(50)]
        public string Username { get; set; }

        public string Mobile { get; set; }

        [DefaultValue(false)]
        public bool MobileConfirmed { get; set; }

        [RegularExpression(
            "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$",
            ErrorMessage = "Incorrect Email format")]
        public string Email { get; set; }

        [DefaultValue(false)]
        public bool EmailConfirmed { get; set; }

        public long EmailConfirmationExpiry { get; set; }

        public string EmailConfirmationSecret { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public long? LastLoginDate { get; set; }

        [StringLength(20)]
        public string RegisterIp { get; set; }

        public long? RegisterDate { get; set; }

        [DefaultValue(0)]
        public int LoginTimes { get; set; }

        [StringLength(20)]
        public string LastLoginIp { get; set; }

        public ActiveStatus Status { get; set; }

        public string Description { get; set; }

        [DefaultValue(false)]
        public bool TwoFactorEnable { get; set; }
     
        public string Picture { get; set; }

        public short? LanguageId { get; set; }

        public Language Language { get; set; }

        public Guid? PersonId { get; set; }

        public Person Person { get; set; }

        public List<Visitor> Visits { get; set; }

        public List<UserSoftware> UserSoftwares { get; set; }

        public List<UserRole> UserRoles { get; set; }

        public List<UserPermission> UserPermissions { get; set; }

        public List<Address> Addresses { get; set; }

        public List<UserSession> UserSessions { get; set; }
    }

    public enum ActiveStatus
    {
        DeActive = 0,
        Active = 1,
        PaymentInfoNotProvided = 2
    }
}
