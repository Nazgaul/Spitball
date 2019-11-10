using Cloudents.Core.DTOs;
using System.Collections.Generic;

namespace Cloudents.Core.Interfaces
{
    public interface IFeedSort
    {
        IEnumerable<FeedDto> SortFeed(IList<FeedDto> itemsFeed, IList<TutorCardDto> tutorsFeed, int page);
    }
}
