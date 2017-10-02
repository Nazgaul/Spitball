using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IMailComponent
    {
        Task GenerateAndSendEmailAsync(string recipient, MailParameters parameters,
            CancellationToken cancellationToken = default(CancellationToken), string category = null);

        Task DeleteUnsubscribeAsync(string email);

        Task<IEnumerable<string>> GetUnsubscribesAsync(DateTime startTime, int page, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<string>> GetInvalidEmailsAsync(DateTime startTime, int page,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<string>> GetBouncesAsync(DateTime startTime, int page,
            CancellationToken cancellationToken = default(CancellationToken));

        Task GenerateSystemEmailAsync(string subject, string text, string to = "ram@cloudents.com");

        Task SendSpanGunEmailAsync(string recipient,
            string ipPool,
            MailParameters parameters,
            int interVal,
            CancellationToken cancellationToken
            );




    }
}
