using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
{
    public interface IDocumentSearch
    {
        Task<(IEnumerable<SearchResult> result, string[] facet)> SearchAsync(SearchQuery model,
            CancellationToken token);
    }

    public interface IFlashcardSearch
    {
        Task<(IEnumerable<SearchResult> result, string[] facet)> SearchAsync(SearchQuery model,
            CancellationToken token);
    }

    public interface IQuestionSearch
    {
        Task<IEnumerable<SearchResult>> SearchAsync(SearchQuery model, CancellationToken token);
    }


    public interface IKeyGenerator
    {
        string GenerateKey(object sourceObject);
    }

    public interface IReadRepository
    {
        Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> getData, CancellationToken token);
    }

    public interface IReadRepositorySingle<T, in TU>
    {
        Task<T> GetAsync(TU query, CancellationToken token);
    }
}
