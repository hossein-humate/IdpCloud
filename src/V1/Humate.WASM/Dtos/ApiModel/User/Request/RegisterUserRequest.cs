using System.ComponentModel.DataAnnotations;

namespace Humate.WASM.Dtos.ApiModel.User.Request
{
    public class RegisterUserRequest
    {
        [RegularExpression(
            "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$",
            ErrorMessage = "Incorrect Email format")]
        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
        
        public string RePassword { get; set; }

        public short? LanguageId { get; set; }
    }
}
