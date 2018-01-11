using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;

namespace Cloudents.Infrastructure.Search
{
    public interface ISearch
    {
        Task<IEnumerable<SearchResult>> DoSearchAsync(SearchModel model, BingTextFormat format,
            CancellationToken token);
    }
}