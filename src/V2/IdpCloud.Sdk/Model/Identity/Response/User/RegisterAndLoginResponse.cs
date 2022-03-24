namespace IdpCloud.Sdk.Model.Identity.Response.User
{
    public class RegisterAndLoginResponse : BaseResponse
    {
        public Identity.User User { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
