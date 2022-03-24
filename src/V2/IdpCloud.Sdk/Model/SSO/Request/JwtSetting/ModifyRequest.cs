using System;

namespace IdpCloud.Sdk.Model.SSO.Request.JwtSetting
{
    public class ModifyRequest
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int ExpireMinute { get; set; }

        public bool HasRefresh { get; set; }

        public int RefreshExpireMinute { get; set; }

        public string Secret { get; set; }

        public Guid SoftwareId { get; set; }
    }
}
