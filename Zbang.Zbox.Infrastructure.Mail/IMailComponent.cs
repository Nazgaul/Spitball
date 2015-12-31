using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IMailComponent
    {
        Task GenerateAndSendEmailAsync(string recipient, MailParameters parameters);
        Task GenerateAndSendEmailAsync(IEnumerable<string> recipients, MailParameters parameters);

        Task FeedbackEmailAsync(string subject, string name, string mailAddress, string feedBack);

        Task DeleteUnsubscribeAsync(string email);
    }
}
