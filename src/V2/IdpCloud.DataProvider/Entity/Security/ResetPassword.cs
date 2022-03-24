using IdpCloud.DataProvider.Entity.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdpCloud.DataProvider.Entity.Security
{
    /// <summary>
    /// Reset Password Entity represent a Request has been delivered to Reset an Account Password(Credential),
    /// which has related activities to store the details about what happend to this request
    /// </summary>
    [Table(name: "ResetPasswords", Schema = "Security")]
    public class ResetPassword : BaseEntity
    {
        public ResetPassword()
        {
            ResetPasswordId = Guid.NewGuid();
        }

        /// <summary>
        /// Primary Key of the Entity
        /// </summary>
        public Guid ResetPasswordId { get; set; }

        /// <summary>
        /// Represent the Expiration Datetime for the Reset password request, Normal value should fill by adding 1 hour to DateTime.Now()
        /// </summary>
        public DateTime Expiry { get; set; }

        /// <summary>
        /// An Auto Genreted value that is useful for validating the Request of recipient and Account Owner
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// The request of reset password is for this user.
        /// Foreign Key of the User Entity
        /// </summary>
        public Guid UserId { get; set; }

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
