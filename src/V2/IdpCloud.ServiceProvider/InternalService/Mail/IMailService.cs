using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IdpCloud.ServiceProvider.InternalService.Mail
{
    public interface IMailService
    {
        Task SendEmail(List<MailAddress> to, string subject, string formattedMessage);
    }
}