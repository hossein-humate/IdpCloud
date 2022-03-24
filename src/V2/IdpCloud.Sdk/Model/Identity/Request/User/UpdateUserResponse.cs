
namespace IdpCloud.Sdk.Model.Identity.Request.User
{
    /// <summary>
    /// Response model for update user controller method
    /// </summary>
    public class UpdateUserResponse : BaseResponse
    {
        /// <summary>
        /// Represent the user record that was updated to the database by current request
        /// </summary>
        public Identity.NewUser User { get; set; }
    }
}
