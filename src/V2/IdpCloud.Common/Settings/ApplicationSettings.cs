
namespace IdpCloud.Common.Settings
{
    public class ApplicationSetting
    {
        /// <summary>
        /// Apllication Name
        /// </summary>
        public string  Name { get; set; }

        /// <summary>
        /// No reply email configs
        /// </summary>
        public string NoReplyEmail { get; set; }

        /// <summary>
        /// SoftwareId for Basemap Identity Provider
        /// </summary>
        public string SoftwareId { get; set; }

        /// <summary>
        /// Administartor Role Id
        /// </summary>
        public string AdministratorRoleId { get; set; }

        /// <summary>
        /// Public user Role Id
        /// </summary>
        public string PublicUserRoleId { get; set; }

        /// <summary>
        /// The allowed origins for CORS requests
        /// </summary>
        public string[] AllowedOrigins { get; set; }
    }
}
