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
    }

    //public interface ISynonymWrite
    //{
    //    Task CreateOrUpdateAsync(string name, string synonyms, CancellationToken token);
    //    void CreateEmpty(string name);
    //}

    public interface ISearchObject
    {
        string Id { get; set; }
    }
}
