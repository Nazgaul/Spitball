using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IReadRepositorySingle<T, in TU>
    {
        Task<T> GetAsync(TU query, CancellationToken token);
    }
}