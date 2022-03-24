using IdpCloud.Sdk.Enum;
using System.ComponentModel;

namespace IdpCloud.Sdk.Model.Identity.Request.User
{
    public class UpdateProfileRequest
    {
        public string Description { get; set; }

        [DefaultValue(false)]
        public bool TwoFactorEnable { get; set; }

        public string Firstname { get; set; }

        public string Middlename { get; set; }

        public string Lastname { get; set; }

        public short? CountryLivingId { get; set; }

        public short? NationalityId { get; set; }

        public short? LanguageId { get; set; }

        public byte[] PictureContent { get; set; }

        public string PictureName { get; set; }

        public GenderType Gender { get; set; }

        public string JobTitle { get; set; }

        public string CompanyName { get; set; }
    }
}
