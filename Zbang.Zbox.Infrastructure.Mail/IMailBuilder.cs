using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IMailBuilder
    {
        void GenerateMail(SendGridMail.ISendGrid message, MailParameters parameters);
    }
}
