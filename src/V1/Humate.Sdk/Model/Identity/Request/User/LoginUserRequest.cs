namespace Humate.Sdk.Model.Identity.Request.User
{
    public class LoginUserRequest
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
        public short? LanguageId { get; set; }
        public string ClientIp { get; set; }
        public string ClientUserAgent { get; set; }
    }
}
