using IdpCloud.DataProvider.Entity.BaseInfo;
using IdpCloud.DataProvider.Entity.Security;
using IdpCloud.DataProvider.Entity.SSO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdpCloud.DataProvider.Entity.Identity
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

        [StringLength(50)]
        public string Username { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Mobile { get; set; }

        [DefaultValue(false)]
        public bool MobileConfirmed { get; set; }

        [RegularExpression(
            "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$",
            ErrorMessage = "Incorrect Email format")]
        public string Email { get; set; }

        [DefaultValue(false)]
        public bool EmailConfirmed { get; set; }

        public DateTime EmailConfirmationExpiry { get; set; }

        public string EmailConfirmationSecret { get; set; }

        public string PasswordHash { get; set; }

        public string PasswordSalt { get; set; }

        public DateTime? LastLoginDate { get; set; }

        [StringLength(20)]
        public string RegisterIp { get; set; }

        public DateTime? RegisterDate { get; set; }

        [DefaultValue(0)]
        public int LoginTimes { get; set; }

        [StringLength(20)]
        public string LastLoginIp { get; set; }

        public UserStatus Status { get; set; }

        public string Description { get; set; }

        [DefaultValue(false)]
        public bool TwoFactorEnable { get; set; }

        public string Picture { get; set; }

        public bool ConfirmedTermAndCondition { get; set; }

        public short? LanguageId { get; set; }

        public Language Language { get; set; }

        /// <summary>
        /// This user will related to the provided OrganisationId
        /// </summary>
        public int? OrganisationId { get; set; }

        /// <summary>
        /// Related Organisation record
        /// </summary>
        public Organisation Organisation { get; set; }

        public List<UserSoftware> UserSoftwares { get; set; }

        public List<UserRole> UserRoles { get; set; }

        public List<UserPermission> UserPermissions { get; set; }

        public List<UserSession> UserSessions { get; set; }

        /// <summary>
        /// Collection of Reset Password request for this user
        /// </summary>
        public List<ResetPassword> ResetPasswords { get; set; }

        /// <summary>
        /// Collection of Security Activity for this user
        /// </summary>
        public List<Activity> Activities { get; set; }
    }

    public enum UserStatus
    {
        DeActive = 0,
        Active = 1
    }
}
