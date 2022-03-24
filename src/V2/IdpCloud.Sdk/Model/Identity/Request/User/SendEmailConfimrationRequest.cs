namespace IdpCloud.Sdk.Model.Identity.Request.User
{
    /// <summary>
    /// A model representing a Send Email Confirmation Link request.
    /// </summary>
    public class SendEmailConfirmationRequest
    {
        /// <summary>
        /// The username or email address.
        /// </summary>
        public string UsernameOrEmail { get; set; }

        /// <summary>
        /// The password value that the user has been Registered before.
        /// </summary>
        public string Password { get; set; }
    }
}
