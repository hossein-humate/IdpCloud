namespace IdpCloud.Sdk.Model.SSO.Response.UserSession
{
    public class RefreshTokenResponse : BaseResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
