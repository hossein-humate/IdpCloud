namespace IdpCloud.DataProvider.Entity.Enum
{
    /// <summary>
    /// Represent List of security activities that could process in SSO application
    /// </summary>
    public enum ActivityType : byte
    {
        /// <summary>
        /// Represent a new Reset Password By Email request came from client
        /// </summary>
        RequestResetPasswordByEmail,

        /// <summary>
        /// Represent an administrator user account set a new password for a user account
        /// </summary>
        AdminChangedPassword,

        /// <summary>
        /// Represent a user try to complete change password process on his own account
        /// </summary>
        ClientChangedPassword,

        /// <summary>
        /// Represent a user try to set a new password on his own account
        /// </summary>
        ClientSetNewPassword,

        /// <summary>
        /// Represent the client rejected the Reset Password Request by any reason
        /// </summary>
        ClientRejectRequestResetPassword,

        /// <summary>
        /// Represent the Email Confirmation Link on register process has been send to the given email address
        /// </summary>
        EmailConfirmationOnRegister,

        /// <summary>
        /// Represent the Email Confirmation Link on register process has been resend to the given email address
        /// </summary>
        ResendEmailConfirmationLink
    }
}
