using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Humate.Sdk.Model.Identity.Request.User
{
    public class UpdateRequest
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string Mobile { get; set; }

        [RegularExpression(
            "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$",
            ErrorMessage = "Incorrect Email format")]
        public string Email { get; set; }

        public string Password { get; set; }

        public ActiveStatus Status { get; set; }

        public string Description { get; set; }

        [DefaultValue(false)]
        public bool TwoFactorEnable { get; set; }

        [DefaultValue(false)]
        public bool EmailConfirmed { get; set; }

        [DefaultValue(false)]
        public bool MobileConfirmed { get; set; }

        public short? LanguageId { get; set; }
    }

    public enum ActiveStatus
    {
        DeActive = 0,
        Active = 1,
        PaymentInfoNotProvided = 2
    }
}
