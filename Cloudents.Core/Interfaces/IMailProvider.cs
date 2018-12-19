using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Request;

namespace Cloudents.Application.Interfaces
{
    public interface IMailProvider
    {

        Task SendSpanGunEmailAsync(
            string ipPool,
            MailGunRequest parameters,
            CancellationToken cancellationToken);

        Task<bool> ValidateEmailAsync(string email, CancellationToken token);

    }

    public interface ISmsProvider
    {
        Task<string> ValidateNumberAsync(string phoneNumber, CancellationToken token);
    }
}