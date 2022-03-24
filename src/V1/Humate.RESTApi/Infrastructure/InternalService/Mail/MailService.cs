using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;

namespace Humate.RESTApi.Infrastructure.InternalService.Mail
{
    public class MailService : IMailService
    {
        private const string CannotFindEmailTemplate = "Cannot find email template in path: {0}";
        private const string AttemptingToSendEMailTemplate = "Attempting to send email";
        private const string EmailHasBeenSendTemplate = "Email has been send";
        private const string ExceptionMessageTemplate = "Exception Thrown sending email";
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailService> _logger;

        public string SendingPayload { get; set; }
        public string EmailConfirmation => "EmailConfirmation.html";
        public string WelcomeUser => "WelcomeUser.html";
        public string LinkRequest => "LinkRequest.html";
        public string InvoiceSepaPayment => "InvoiceSepaPayment.html";
        public string InformationalReportResultUser => "InformationalReportResultUser.html";
        public string InformationalReportResultRecipient => "InformationalReportResultRecipient.html";

        public MailService
        (
            IConfiguration configuration,
            ILogger<MailService> logger
        )
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendMailAsync(string template, string[] receiverEmails, string[] ccEmails,
            string[] bccEmails, string subject, string[] attachments = default, params object[] parameters)
        {
            try
            {
                var path = GetPath(template);
                if (!System.IO.File.Exists(path))
                {
                    _logger.LogError(CannotFindEmailTemplate, path);
                    return false;
                }

                var formattedMessage = GetFormattedMessage(parameters, path);
                _logger.LogInformation(AttemptingToSendEMailTemplate);
                using (var smtpClient = GetSmtpClient())
                {
                    var message = attachments == default ? GetMessage(subject, formattedMessage) :
                        GetMessageWithAttachment(subject, formattedMessage, attachments);
                    foreach (var receiverEmail in receiverEmails)
                    {
                        message.To.Add(new MailAddress(receiverEmail, _configuration["Application:Name"]));
                    }

                    foreach (var ccEmail in ccEmails)
                    {
                        message.CC.Add(new MailAddress(ccEmail, _configuration["Application:Name"]));
                    }

                    foreach (var bccEmail in bccEmails)
                    {
                        message.Bcc.Add(new MailAddress(bccEmail, _configuration["Application:Name"]));
                    }
                    smtpClient.SendCompleted += SendCompletedCallback;
                    await smtpClient.SendMailAsync(message);
                    _logger.LogInformation(EmailHasBeenSendTemplate);
                }

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, ExceptionMessageTemplate);
                return false;
            }
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            SendingPayload = e.Error != null ? $"{e.Error}" : "Successfully Sent.";
        }

        private static string GetFormattedMessage(object[] parameters, string path)
        {
            var body = System.IO.File.ReadAllText(path);
            return string.Format(body, parameters);
        }

        private static string GetPath(string template)
        {
            return string.Format("{0}{1}wwwroot{1}EmailTemplate{1}{2}", Environment.CurrentDirectory,
                Path.DirectorySeparatorChar, template);
        }

        private MailMessage GetMessage(string subject, string formattedMessage,
            bool isBodyHtml = true)
        {
            return new MailMessage
            {
                From = new MailAddress(_configuration["MailService:Username"], _configuration["Application:Name"]),
                Subject = subject,
                Body = formattedMessage,
                IsBodyHtml = isBodyHtml
            };
        }

        private MailMessage GetMessageWithAttachment(string subject, string formattedMessage,string[] attachments,
            bool isBodyHtml = true)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_configuration["MailService:Username"], _configuration["Application:Name"]),
                Subject = subject,
                Body = formattedMessage,
                IsBodyHtml = isBodyHtml
            };

            foreach (var item in attachments)
            {
                mail.Attachments.Add(new Attachment(item));
            }

            return mail;
        }

        private SmtpClient GetSmtpClient()
        {
            return new SmtpClient
            {
                Host = _configuration["MailService:Host"],
                Port = int.Parse(_configuration["MailService:Port"]),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Timeout = 100000,
                Credentials = GetNetworkCredential(),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
        }

        private NetworkCredential GetNetworkCredential()
        {
            return new NetworkCredential
            {
                UserName = _configuration["MailService:Username"],
                Password = _configuration["MailService:Password"]
            };
        }

        #region Dispose
        private readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _handle.Dispose();
            }

            _disposed = true;
        }
        #endregion
    }
}
