namespace Humate.Sdk.Model.Identity.Response.User
{
    public class LoginUserResponse : BaseResponse
    {
        public Identity.User User { get; set; }
        public string Token { get; set; }
    }
}
