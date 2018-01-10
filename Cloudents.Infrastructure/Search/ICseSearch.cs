using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Infrastructure.Search
{
    public interface ISearch
    {
        Task<IEnumerable<SearchResult>> DoSearchAsync(SearchModel model,
            CancellationToken token);
    }
}