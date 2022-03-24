﻿namespace IdpCloud.Sdk.Model.SSO.Request.Organisation
{
    /// <summary>
    /// Request Model for Editing a existing record of organisation in Database
    /// </summary>
    public class UpdateRequest
    {
        /// <summary>
        /// Represent the Organisation Id
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
