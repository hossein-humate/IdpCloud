using IdpCloud.Sdk.Model.Identity;
using IdpCloud.Sdk.Model.SSO;

namespace IdpCloud.Sdk.Auth
{
    /// <summary>
    /// Authenticated User context represent all required information about current authenticated account
    /// </summary>
    public class AuthUser : IAuthUser
    {
        /// <summary>
        /// Represent the authenticated User 
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Represent the current active user session 
        /// </summary>
        public UserSession CurrentSession { get; set; }

        /// <summary>
        /// Represent the authenticated users Organisation 
        /// </summary>
        public Organisation Organisation { get; set; }

        /// <summary>
        /// Represent the authenticated users Role 
        /// </summary>
        public Role Role { get; set; }
    }
}
