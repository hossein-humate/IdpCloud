using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Humate.Sdk.Model.Identity.Request.User
{
    public class CreateRequest
    {
        [Required]
        public Guid SoftwareId { get; set; }

        public Guid RoleId { get; set; }

        [Required]
        public string Username { get; set; }

        public string Mobile { get; set; }

        [RegularExpression(
            "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$",
            ErrorMessage = "Incorrect Email format")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public ActiveStatus Status { get; set; }

        public string Description { get; set; }

        public string RegisterIp { get; set; }

        [DefaultValue(false)]
        public bool TwoFactorEnable { get; set; }

        [DefaultValue(false)]
        public bool EmailConfirmed { get; set; }

        [DefaultValue(false)]
        public bool MobileConfirmed { get; set; }

        public short CountryLivingId { get; set; }
        
        public short NationalityId { get; set; }

        public string Address { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public short? LanguageId { get; set; }
    }
}
