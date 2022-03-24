namespace Humate.WASM.Dtos.ApiModel.User.Response
{
    public class RegisterAndLoginResponse : BaseResponse
    {
        public UserApiModel User { get; set; }
        public string Token { get; set; }
    }
}
