namespace IdpCloud.Sdk.Model.Identity.Request.User
{
    /// <summary>
    /// Request model to map all incoming json data, contain parameters those are needed to Register a new User in the Identity Provider
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// Represent the Username of the Account
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Represent the Email Address that should belong to User
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Represent the Password value for this new account
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Represent the Firstname value
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// Represent the Lastname value
        /// </summary>
        public string Lastname { get; set; }

        /// <summary>
        /// Represent the Name of Organisation
        /// </summary>
        public string OrganisationName { get; set; } 

        /// <summary>
        /// Represent the Billing Email Address of Organisation
        /// </summary>
        public string OrganisationBillingEmail { get; set; } 

        /// <summary>
        /// Represent the Phone Number of Organisation
        /// </summary>
        public string OrganisationPhone { get; set; } 

        /// <summary>
        /// Represent the Billling Full Address of Organisation
        /// </summary>
        public string OrganisationBillingAddress { get; set; } 

        /// <summary>
        /// Represent VAT Number of Organisation
        /// </summary>
        public string OrganisationVatNumber { get; set; } 
    }
}
