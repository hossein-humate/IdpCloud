namespace IdpCloud.Sdk.Model.SSO.Request.UserSession
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
        public string ClientIp { get; set; }
        public string ClientUserAgent { get;set; }
    }
}
