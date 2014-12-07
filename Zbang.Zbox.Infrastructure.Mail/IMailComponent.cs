﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IMailComponent
    {
        void GenerateAndSendEmail(string recipient, MailParameters parameters);
        void GenerateAndSendEmail(IEnumerable<string> recipients, MailParameters parameters);

        void GenerateAndSendEmail(IEnumerable<string> recipients, string mailContent);

        Task DeleteUnsubscribe(string email);
    }
}
