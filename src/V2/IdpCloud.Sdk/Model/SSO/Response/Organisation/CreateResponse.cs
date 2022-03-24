namespace IdpCloud.Sdk.Model.SSO.Response.Organisation
{
    /// <summary>
    /// Response model for Create Organisation controller method
    /// </summary>
    public class CreateResponse : BaseResponse
    {
        /// <summary>
        /// Represent the Organisation record that was added to the database by current request
        /// </summary>
        public Identity.Organisation Organisation { get; set; }
    }
}
