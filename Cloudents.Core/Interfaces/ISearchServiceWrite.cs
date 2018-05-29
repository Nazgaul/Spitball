using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface ISearchServiceWrite<in T> where T : class, ISearchObject, new()
    {
        Task UpdateDataAsync(IEnumerable<T> items, CancellationToken token);

        Task DeleteDataAsync(IEnumerable<string> ids, CancellationToken token);
        Task CreateOrUpdateAsync(CancellationToken token);
        Task UpdateDataAsync(IEnumerable<T> items, IEnumerable<string> ids, CancellationToken token);
    }

    public interface ISearchObject
    {
        string Id { get; set; }
    }
}
