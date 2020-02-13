using Cloudents.Core.DTOs.Feed;
using Cloudents.Core.Query.Feed;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IFeedService
    {
        Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedQuery query, CancellationToken token);

        Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedWithCourseQuery query, CancellationToken token);

        Task<IEnumerable<FeedDto>> GetFeedAsync(SearchFeedQuery query, CancellationToken token);
    }


    public interface IFeedTypeService
    {
        Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedQuery query, CancellationToken token);
        Task<IEnumerable<FeedDto>> GetFeedAsync(GetFeedWithCourseQuery query, CancellationToken token);

        Task<IEnumerable<FeedDto>> GetFeedAsync(SearchFeedQuery query, CancellationToken token);
       
    }
}
