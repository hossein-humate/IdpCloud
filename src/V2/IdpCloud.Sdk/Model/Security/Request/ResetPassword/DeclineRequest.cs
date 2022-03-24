namespace IdpCloud.Sdk.Model.Security.Request.ResetPassword
{
    /// <summary>
    /// Request model to map all incoming json data of the Decline API
    /// </summary>
    public class DeclineRequest
    {
        /// <summary>
        /// Encoded Key that is provided in the ResetPassword Email 
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Secure random string value, provided in the ResetPassword Email
        /// </summary>
        public string Secret { get; set; }
    }
}
