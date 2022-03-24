namespace IdpCloud.Sdk.Model.SSO.Response.JwtSetting
{
    public class GetBySoftwareIdResponse : BaseResponse
    {
        public SSO.JwtSetting JwtSetting { get; set; }
        public Identity.Software Software { get; set; }
    }
}
