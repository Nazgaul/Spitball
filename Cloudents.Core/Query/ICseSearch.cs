using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Enum;

namespace Cloudents.Application.Query
{
    public interface ISearch
    {
        Task<IEnumerable<SearchResult>> SearchAsync(SearchModel model,int page, HighlightTextFormat format,
            CancellationToken token);
    }
}