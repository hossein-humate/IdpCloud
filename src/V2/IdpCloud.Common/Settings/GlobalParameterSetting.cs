
using System;

namespace IdpCloud.Common.Settings
{
    public class GlobalParameterSetting
    {
        /// <summary>
        /// The sign in Url for frontend
        /// </summary>
        public string SignInUrl { get; set; }

        /// <summary>
        /// The  ConfirmEmail Url for frontend
        /// </summary>
        public string SsoConfirmEmailUrl { get; set; }

        /// <summary>
        /// The SSO web Application SoftwareId
        /// </summary>
        public Guid SsoSoftwareId { get; set; }

        /// <summary>
        /// The SSO Public and default RoleId
        /// </summary>
        public Guid SsoPublicRoleId { get; set; }

        /// <summary>
        /// The change Password Url for frontend
        /// </summary>
        public string SsoChangePasswordUrl { get; set; }

        /// <summary>
        /// The RejectReset password url for frontend
        /// </summary>
        public string SsoRejectResetPasswordUrl { get; set; }

        /// <summary>
        /// The storage path
        /// </summary>
        public string StoragePath { get; set; }

    }
}
