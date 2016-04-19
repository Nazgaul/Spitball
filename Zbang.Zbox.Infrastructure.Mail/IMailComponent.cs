﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IMailComponent
    {
        Task GenerateAndSendEmailAsync(string recipient, MailParameters parameters, CancellationToken cancellationToken = default(CancellationToken));
        //Task GenerateAndSendEmailAsync(IEnumerable<string> recipients, MailParameters parameters);


        Task DeleteUnsubscribeAsync(string email);

        Task<IEnumerable<string>> GetUnsubscribesAsync(DateTime startTime, int page, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<string>> GetInvalidEmailsAsync(DateTime startTime, int page,
            CancellationToken cancellationToken = default(CancellationToken));

        Task GenerateSystemEmailAsync(string text);


    }
}
