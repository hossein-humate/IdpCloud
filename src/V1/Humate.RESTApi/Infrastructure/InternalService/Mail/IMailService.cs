using System;
using System.Threading.Tasks;

namespace Humate.RESTApi.Infrastructure.InternalService.Mail
{
    public interface IMailService : IDisposable
    {
        string SendingPayload { get; }
        string EmailConfirmation { get; }
        string WelcomeUser { get; }
        string LinkRequest { get; }
        string InvoiceSepaPayment { get; }
        string InformationalReportResultUser { get; }
        string InformationalReportResultRecipient { get; }

        Task<bool> SendMailAsync(string template, string[] receiverEmails, string[] ccEmails,
            string[] bccEmails, string subject, string[] attachments = default, params object[] parameters);
    }
}