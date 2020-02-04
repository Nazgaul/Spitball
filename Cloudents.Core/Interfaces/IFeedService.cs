using Cloudents.Core.DTOs;
using Cloudents.Core.Query.Feed;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IFeedService
    {
        Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedQuery query, CancellationToken token);

        Task<IEnumerable<FeedDto>> GetFeedAsync(SearchFeedQuery query, CancellationToken token);
    }
}
