using System;

namespace IdpCloud.Sdk.Model.SSO.Response.Organisation
{
    /// <summary>
    /// Model provide all parameters needed in clients side Data Grid view
    /// </summary>
    public class OrganisationSummary : Identity.Organisation
    {
        /// <summary>
        /// Number Of users in this organisation
        /// </summary>
        public int UserCounts { get; set; }

        /// <summary>
        /// Date and time of creation this organisaion
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Represent the last datetime that this organisation changed
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Last Login date of any user in this organisation
        /// </summary>
        public DateTime? LastLoginDate { get; set; }
    }
}
