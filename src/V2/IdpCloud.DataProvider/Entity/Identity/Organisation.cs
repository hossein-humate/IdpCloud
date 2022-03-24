using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdpCloud.DataProvider.Entity.Identity
{
    /// <summary>
    /// Organisation entity represent the detail of customer companies for Basemap Identity Provider
    /// </summary>
    [Table(name: "Organisations", Schema = "Identity")]
    public class Organisation : BaseEntity
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

        /// <summary>
        /// List of users already exist in this Organisation
        /// </summary>
        public List<User> Users { get; set; }
    }
}