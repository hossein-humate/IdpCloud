namespace IdpCloud.Sdk.Model.Security.Request.ResetPassword
{
    /// <summary>
    /// Model to accept new password request from clients
    /// </summary>
    public class NewPasswordRequest
    {
        /// <summary>
        /// Represenet the old password value
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// Represent the new password value
        /// </summary>
        public string NewPassword { get; set; }
    }
}
