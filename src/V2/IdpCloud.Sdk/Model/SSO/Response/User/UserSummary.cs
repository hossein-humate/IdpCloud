using System;

namespace IdpCloud.Sdk.Model.SSO.Response.Organisation
{
    /// <summary>
    /// Model provide all parameters needed in clients side Data Grid view to listing Users
    /// </summary>
    public class UserSummary : Identity.User
    {
        /// <summary>
        /// Represent the Fullname
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represent the Organisation Name that the current user exist in
        /// </summary>
        public string Organisation { get; set; }

        /// <summary>
        /// Represent the Role Name that the current user already has in SSO
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Date and time of creation for this user
        /// </summary>
        public DateTime CreationDate { get; set; }
    }
}
