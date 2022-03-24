using IdpCloud.DataProvider.Entity.Enum;
using IdpCloud.DataProvider.Entity.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdpCloud.DataProvider.Entity.Security
{
    /// <summary>
    /// Activity Entity represent the operation has been occured on a security action like(Resetting password,Change password, Request Two Factor, ...)
    /// </summary>
    [Table(name: "Activities", Schema = "Security")]
    public class Activity : BaseEntity
    {
        public Activity()
        {
            ActivityId = Guid.NewGuid();
        }

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
        /// Activity may related to this user, or operator of the current activity may not recognized yet and it should be null.
        /// Foreign Key of the User Entity
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// This class will load the User data if Parameter UserId has value.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// If the current activity is ResetPassword this parameter must filled by related request reset password.
        /// </summary>
        public Guid? ResetPasswordId { get; set; }

        public ResetPassword ResetPassword { get; set; }

        /// <summary>
        /// Represent the details about this activity, for example the reason for activity Type = ClientRejectRequestResetPassword required to fill in the parameter.
        /// </summary>
        public string Decription { get; set; }
    }
}
