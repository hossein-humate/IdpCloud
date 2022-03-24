using IdpCloud.Sdk.Enum;
using IdpCloud.Sdk.Model.Identity;
using System;

namespace IdpCloud.Sdk.Model.Security
{
    /// <summary>
    /// Activity DTO represent the operation has been occured on a security action like(Resetting password,Change password, Request Two Factor, ...)
    /// </summary>
    public class Activity
    {
        /// <summary>
        /// Primary Key of the Entity
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// Client Request Ip Address
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Reperesent the HTTP User-Agent Request Header parameter value
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Represent the Type of activity that should exist in the Enum list
        /// </summary>
        public ActivityType Type { get; set; }

        /// <summary>
        /// This class will load the User data if Parameter UserId has value.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// If the current activity is ResetPassword this Model must filled by related request reset password.
        /// </summary>
        public ResetPassword ResetPassword { get; set; }

        /// <summary>
        /// Represent the details about this activity, for example the reason for activity Type = ClientRejectRequestResetPassword required to fill in the parameter.
        /// </summary>
        public string Decription { get; set; }
    }
}
