﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IMailComponent
    {
        Task GenerateAndSendEmailAsync(string recipient, MailParameters parameters,
            CancellationToken cancellationToken = default(CancellationToken), string category = null);
        //Task GenerateAndSendEmailAsync(IEnumerable<string> recipients, MailParameters parameters);


        Task DeleteUnsubscribeAsync(string email);

        Task<IEnumerable<string>> GetUnsubscribesAsync(DateTime startTime, int page, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<string>> GetInvalidEmailsAsync(DateTime startTime, int page,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<string>> GetBouncesAsync(DateTime startTime, int page,
            CancellationToken cancellationToken = default(CancellationToken));

        Task GenerateSystemEmailAsync(string subject, string text);

        Task SendSpanGunEmailAsync(string recipient, string ipPool,
            string body, string subject,
            string name, string category, string universityUrl);






    }
}
