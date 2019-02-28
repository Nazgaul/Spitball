using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
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
        Task<(string phoneNumber, string country)> ValidateNumberAsync(string phoneNumber, CancellationToken token);
    }
}