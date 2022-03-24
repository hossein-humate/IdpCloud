using IdpCloud.Sdk.Enum;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdpCloud.Sdk.Model.Identity.Request.User
{
    public class CreateRequest
    {
        public Guid SoftwareId { get; set; }

        public Guid RoleId { get; set; } = Guid.Empty;

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

        public string Password { get; set; }

        public UserStatus Status { get; set; }

        public string Description { get; set; }

        [DefaultValue(false)]
        public bool TwoFactorEnable { get; set; }

        public bool ConfirmedTermAndCondition { get; set; }

        public short? LanguageId { get; set; }

        public Guid? PersonId { get; set; }

        public string Firstname { get; set; }

        public string Middlename { get; set; }

        public string Lastname { get; set; }

        public short? CountryLivingId { get; set; }

        public short? NationalityId { get; set; }

        public byte[] PictureContent { get; set; }

        public string PictureName { get; set; }

        public GenderType Gender { get; set; }

        public string JobTitle { get; set; }

        public string CompanyName { get; set; }
    }
}
