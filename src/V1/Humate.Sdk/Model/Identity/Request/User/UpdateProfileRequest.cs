using System.ComponentModel;

namespace Humate.Sdk.Model.Identity.Request.User
{
    public class UpdateProfileRequest
    {
        public string Mobile { get; set; }

        [DefaultValue(false)]
        public bool TwoFactorEnable { get; set; }

        public short? LanguageId { get; set; }
    }
}
