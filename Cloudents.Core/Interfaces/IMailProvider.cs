using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IMailProvider
    {
        Task<bool> ValidateEmailAsync(string email, CancellationToken token);
    }
}