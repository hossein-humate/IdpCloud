namespace IdpCloud.Sdk.Model.Identity.Response.User
{
    /// <summary>
    /// Response model for login API
    /// </summary>
    public class LoginResponse : BaseResponse
    {
        /// <summary>
        /// Represent the authenticated user account
        /// </summary>
        public Identity.User User { get; set; }

        /// <summary>
        /// Represent the authenticated users Organisation
        /// </summary>
        public Organisation Organisation { get; set; }

        /// <summary>
        /// Represent the authenticated users Role 
        /// </summary>
        public Identity.Role Role { get; set; }

        /// <summary>
        /// Represent the secret value that contain user claims regarding to OAuth2 standard
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Represent a refresh token to generate a new JWT for authenticated user without calling Login operation again
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
