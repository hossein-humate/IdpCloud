
namespace IdpCloud.Sdk.Model.Identity.Request.User
{
    /// <summary>
    /// Request model to map all incoming json data, contain parameters those are needed to add a new User in the Identity Provider
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// Represent the Firstname 
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// Represent the Lastname 
        /// </summary>
        public string Lastname { get; set; }

        /// <summary>
        /// Represent the Username of the Account
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Represent the Email Address that should belong to User
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Represent the Phone number that should belong to User
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Represnts the OrganisationId to which user belongs
        /// </summary>
        public int  OrganisationId { get; set; }

        /// <summary>
        /// Represnts the RoleId of Role assigned to user
        /// </summary>
        public string RoleId { get; set; }

    }
}
