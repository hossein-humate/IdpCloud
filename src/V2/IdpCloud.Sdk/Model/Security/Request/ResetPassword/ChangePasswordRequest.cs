namespace IdpCloud.Sdk.Model.Security.Request.ResetPassword
{
    /// <summary>
    /// Request model to map all incoming json data of the ChangePassword API
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// Encoded Key that is provided in the ResetPassword Email 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Secure random string value, provided in the ResetPassword Email
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Contain the new Password value to replace with the old one
        /// </summary>
        public string Password { get; set; }
    }
}
