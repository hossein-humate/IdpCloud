using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.InternalService.Mail
{
    /// <summary>
    /// A Email service
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Html Template name for EmailConfirmation 
        /// </summary>
        string EmailConfirmation { get; }

        /// <summary>
        /// Html Template name for Welcome User
        /// </summary>
        string WelcomeUser { get; }

        /// <summary>
        /// Html template name for Reset password
        /// </summary>
        string ResetPassword { get; }

        /// <summary>
        /// Html template name for New user registered by Admin
        /// </summary>
        string NewUserRegister { get; }

        /// <summary>
        /// Sends web email
        /// </summary>
        /// <param name="template">Html Template to use for sending email</param>
        /// <param name="toEmailAddress">Contains the To email address</param>
        /// <param name="subject">The mail subject</param>
        /// <param name="attachments">Attachments</param>
        /// <param name="parameters">Parameters to send</param>
        /// <returns><see cref="Task<bool>"></see> True if success else false</returns>
        Task<bool> SendEmail(string template,
            string[] toEmailAddress,
            string subject,
            string[] attachments = default,
            params object[] parameters);
    }
}
