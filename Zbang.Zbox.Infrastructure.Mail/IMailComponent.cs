using System.Collections.Generic;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IMailComponent
    {
        void GenerateAndSendEmail(string recepient, MailParameters parameters);
        void GenerateAndSendEmail(IEnumerable<string> recepients, MailParameters parameters);
    }
}
