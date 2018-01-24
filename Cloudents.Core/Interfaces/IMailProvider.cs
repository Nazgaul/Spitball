using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IMailProvider
    {
        Task GenerateSystemEmailAsync(string subject, string text, CancellationToken token);
    }
}