namespace IdpCloud.Sdk.Enum
{
    /// <summary>
    /// All acceptable Status for a User account
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// Represent the Not Active status of user account
        /// </summary>
        /// <remarks>
        /// This status means this account cannot login or use any functionality in the SSO plaform if currently 
        /// the <see cref="Model.Identity.User.Status"/> parameter assigned to this value.
        /// </remarks>
        DeActive = 0,

        /// <summary>
        /// Represent the normal and Active status of the user account
        /// </summary>
        Active = 1
    }
}
