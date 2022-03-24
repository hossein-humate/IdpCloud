namespace Humate.Sdk.Model.Identity.Response.User
{
    public class GenerateJwtResponse : BaseResponse
    {
        public Identity.User User { get; set; }
        public string Token { get; set; }
    }
}
