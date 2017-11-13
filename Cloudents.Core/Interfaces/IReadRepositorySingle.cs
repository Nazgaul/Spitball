using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IReadRepositoryAsync<T, in TU>
    {
        Task<T> GetAsync(TU query, CancellationToken token);

    }

    public interface IReadRepository<out T, in TU>
    {
        T Get(TU query);

    }

    public interface IReadRepositoryAsync<T>
    {
        Task<T> GetAsync(CancellationToken token);
    }
}