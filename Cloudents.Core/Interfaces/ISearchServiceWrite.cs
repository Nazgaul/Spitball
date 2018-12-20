using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ISearchServiceWrite<in T> where T : class, ISearchObject, new()
    {
        Task<bool> UpdateDataAsync(IEnumerable<T> items, CancellationToken token);

        Task<bool> DeleteDataAsync(IEnumerable<string> ids, CancellationToken token);
        Task CreateOrUpdateAsync(CancellationToken token);
        Task<bool> UpdateDataAsync(IEnumerable<T> items, IEnumerable<string> ids, CancellationToken token);
    }
}
