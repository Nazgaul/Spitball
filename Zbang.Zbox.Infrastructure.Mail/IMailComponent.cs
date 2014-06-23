using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IMailComponent
    {
        void GenerateAndSendEmail(string recepient, MailParameters parameters);
        void GenerateAndSendEmail(IEnumerable<string> recepients, MailParameters parameters);

        Task DeleteUnsubscribe(string email);
    }
}
