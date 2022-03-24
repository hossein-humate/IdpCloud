using System;

namespace IdpCloud.Sdk.Model.SSO.Response.UserSoftware
{
    /// <summary>
    /// Data model provide software parameters without any security information
    /// </summary>
    public class SoftwareDto
    {
        /// <summary>
        /// Represent softwareId
        /// </summary>
        public Guid SoftwareId { get; set; }

        /// <summary>
        /// Represent general name for this software
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Provide information about what is this software
        /// </summary>
        public string BusinessDescription { get; set; }

        /// <summary>
        /// Represent the Base Url or execution path for this software
        /// </summary>
        public string UrlOrPath { get; set; }
    }
}
