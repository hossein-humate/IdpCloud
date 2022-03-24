
namespace IdpCloud.Sdk.Model.Identity.Response.User
{
    /// <summary>
    /// Response model for Create User controller method
    /// </summary>
    public class CreateUserResponse : BaseResponse
    {
        /// <summary>
        /// Represent the User record that was added to the database by current request
        /// </summary>
         public Identity.NewUser User { get; set; }
    }
}
