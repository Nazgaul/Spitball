using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;

namespace Cloudents.Infrastructure.Search
{
    public interface ICseSearch
    {
        Task<IEnumerable<SearchResult>> DoSearchAsync(CseModel model,
            CancellationToken token);
    }
}