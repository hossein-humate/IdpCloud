using IdpCloud.Sdk.Model.Identity;
using System;

namespace IdpCloud.Sdk.Model.SSO
{
    public class JwtSetting
    {
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
