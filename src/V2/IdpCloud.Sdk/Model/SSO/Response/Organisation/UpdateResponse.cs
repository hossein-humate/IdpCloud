namespace IdpCloud.Sdk.Model.SSO.Response.Organisation
{
    /// <summary>
    /// Response model for Create Organisation controller method
    /// </summary>
    public class UpdateResponse : BaseResponse
    {
        /// <summary>
        /// Represent the Organisation record that was updated to the database by current request
        /// </summary>
        public Identity.Organisation Organisation { get; set; }
    }
}
