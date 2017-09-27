using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ITitleSearch
    {
        Task<string> SearchAsync(string term, CancellationToken token);
    }
}