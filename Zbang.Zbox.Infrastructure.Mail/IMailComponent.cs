using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IMailComponent
    {
        Task GenerateAndSendEmailAsync(string recipient, MailParameters parameters);
        Task GenerateAndSendEmailAsync(IEnumerable<string> recipients, MailParameters parameters);

        Task GenerateAndSendEmailAsync(IEnumerable<string> recipients, string mailContent);

        Task DeleteUnsubscribeAsync(string email);
    }
}
