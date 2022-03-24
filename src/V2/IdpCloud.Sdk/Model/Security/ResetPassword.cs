using IdpCloud.Sdk.Model.Identity;
using System;
using System.Collections.Generic;

namespace IdpCloud.Sdk.Model.Security
{
    /// <summary>
    /// Reset Password DTO represent a Request has been delivered to Reset an Account Password(Credential),
    /// which has related activities to store the details about what happend to this request
    /// </summary>
    public class ResetPassword
    {
        /// <summary>
        /// Primary Key of the Entity
        /// </summary>
        public Guid ResetPasswordId { get; set; }

        /// <summary>
        /// Represent the Expiration Datetime for the Reset password request, Normal value should fill by adding 1 hour to DateTime.Now()
        /// </summary>
        public DateTime Expiry { get; set; }

        /// <summary>
        /// This class will load the User data.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Represent the Request when Active = true it means nothing happend yet to this Reset Password Request and when Active = false represent that it was Rejected, Completed or Expired
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Related Activies to this Reset Password request will be load in this List.
        /// </summary>
        public List<Activity> Activities { get; set; }
    }
}
