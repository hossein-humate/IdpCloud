namespace Humate.WASM.Dtos.ApiModel.User.Request
{
    public class LoginUserRequest
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
        public short? LanguageId { get; set; }
    }
}
