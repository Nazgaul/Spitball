using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Core.Query.Feed
{
    public class SearchFeedQuery
    {
        public SearchFeedQuery(UserProfile profile, string term, int page, FeedType? filter, string country, string? course)
        {
            Profile = profile;
            Term = term;
            Page = page;
            Filter = filter ?? FeedType.All;
            Country = country;
            Course = course;
        }
        public UserProfile Profile { get; }
        public string Term { get;  }
        public int Page { get; }
        public FeedType Filter { get; }
        public string Country { get; }
        public string? Course { get; }
    }
}
