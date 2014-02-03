using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IMailComponent
    {
        void GenerateAndSendEmail(string recepient, MailParameters parameters);
    }
}
