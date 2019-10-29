using Cloudents.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Interfaces
{
    public interface IFeedSort
    {
        IEnumerable<FeedDto> SortFeed(IList<FeedDto> itemsFeed, IList<TutorCardDto> tutorsFeed, int page);
    }
}
