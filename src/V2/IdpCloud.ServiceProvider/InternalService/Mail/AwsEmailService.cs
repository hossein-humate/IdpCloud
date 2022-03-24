using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.InternalService.Mail
{
    public class AwsEmailService : IEmailService
    {
        private readonly IAwsMailServiceProvider _awsMailServiceProvider;
        private readonly ILogger<AwsEmailService> _logger;
        public string EmailConfirmation => "EmailConfirmation.html";
        public string WelcomeUser => "WelcomeUser.html";
        public string ResetPassword => "ResetPassword.html";
        public string NewUserRegister => "NewEmailForUserAddedByAdmin.html";
        /// <summary>
        /// Instantiates a new instance of <see cref="AwsEmailService"/>.
        /// </summary>
        /// <param name="awsMailServiceProvider">An instance of <see cref="IAwsMailServiceProvider"/> to inject.</param>
        public AwsEmailService(IAwsMailServiceProvider awsMailServiceProvider,
            ILogger<AwsEmailService> logger)

        {
            _awsMailServiceProvider = awsMailServiceProvider ?? throw new ArgumentNullException(nameof(awsMailServiceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        ///<inheritdoc/>
        public async Task<bool> SendEmail(string templateName,
                string[] toEmailAddress,
                string subject,
                string[] attachments = default,
                params object[] parameters)
        {

            try
            {
                var formattedMessage = GetFormattedMessage(parameters, templateName);
                _logger.LogInformation("Attempting to send email");

                var awsMailService = _awsMailServiceProvider.Create();
                var to = new List<MailAddress>() { new MailAddress(toEmailAddress.FirstOrDefault()) };
               // await awsMailService.SendEmail(to, subject, formattedMessage);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception Thrown sending email");
                return false;
            }
        }
        private static string GetFormattedMessage(object[] parameters, string templateName)
        {
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("IdpCloud.ServiceProvider.Resources." + templateName));
            var templateContent = reader.ReadToEnd();
            return string.Format(templateContent, parameters);
        }
    }
}
