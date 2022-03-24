using System;

namespace IdpCloud.Sdk.Model.Identity
{
    public class NewUser
    {
        /// <summary>
        /// Represent the Id of the User
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Represent the UserName of the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Represent the FirstName of the user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Represent the LastName of the user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Represent the Mobile of the user
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Represent the Email of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Represent the Id of the Organisation to which user is assigned
        /// </summary>
        public int OrganisationId { get; set; }

    }
}
