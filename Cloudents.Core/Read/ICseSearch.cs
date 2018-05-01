using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;

namespace Cloudents.Core.Read
{
    public interface ISearch
    {
        Task<IEnumerable<SearchResult>> SearchAsync(SearchModel model,int page, HighlightTextFormat format,
            CancellationToken token);
    }
}