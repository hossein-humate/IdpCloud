namespace IdpCloud.Sdk.Model.Identity.Request.User
{
    /// <summary>
    /// A model representing a login request.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// The login attempt username.
        /// </summary>
        public string UsernameOrEmail { get; set; }

        /// <summary>
        /// The login attempt password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The selected language the user requests.
        /// </summary>
        public short? LanguageId { get; set; }
    }
}
