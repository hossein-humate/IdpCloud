using Microsoft.Extensions.Options;

namespace IdpCloud.Common.Settings
{
    /// <summary>
    /// Email Service setting
    /// </summary>
    public class MailServiceSetting : IOptions<SmtpConfig>
    {
        /// <summary>
        /// Sender email address
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// Sender name
        /// </summary>
        public string FromName { get; set; }

        public SmtpConfig Value => new SmtpConfig()
        {
            FromName = FromName,
            FromAddress = FromAddress
        };
    }
}
