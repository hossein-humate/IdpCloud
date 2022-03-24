namespace IdpCloud.Sdk.Model.Identity
{
    /// <summary>
    /// Represent the Organisation DTO to initialize an instans of Ornganization entity
    /// </summary>
    public class Organisation
    {
        /// <summary>
        /// Represent the Id of the Organisation
        /// </summary>
        public int OrganisationId { get; set; }

        /// <summary>
        /// Represent the Name of the Organisation
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represent the Billing Email Address of this Organisation
        /// </summary>
        public string BillingEmail { get; set; }

        /// <summary>
        /// Represent the Phone Number of this Organisation
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Represent the Billling Full Address of this Organisation
        /// </summary>
        public string BillingAddress { get; set; }

        /// <summary>
        /// Represent VAT Number of this Organisation
        /// </summary>
        public string VatNumber { get; set; }
    }
}
