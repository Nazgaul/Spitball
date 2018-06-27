using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
{
    public interface IMailProvider
    {
        //Task GenerateSystemEmailAsync(string subject, string text, CancellationToken token);

        Task SendSpanGunEmailAsync(
            string ipPool,
            MailGunRequest parameters,
            CancellationToken cancellationToken);

        //Task SendEmailAsync(string email, string subject, string message, CancellationToken token);
    }
}