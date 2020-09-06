using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ISmsProvider
    {
        Task<string> SendSmsAsync(string message, string phoneNumber, CancellationToken token);
    }


}