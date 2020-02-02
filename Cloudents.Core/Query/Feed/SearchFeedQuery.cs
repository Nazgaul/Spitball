using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Cloudents.Core.Query.Feed
{
    public class SearchFeedQuery
    {
        public SearchFeedQuery(UserProfile profile, string term, int page, FeedType? filter, string country, string course)
        {
            Profile = profile;
            Term = term;
            Page = page;
            Filter = filter;
            Country = country;
            Course = course;
        }
        public UserProfile Profile { get; set; }
        public string Term { get; set; }
        public int Page { get; set; }
        public FeedType? Filter { get; set; }
        public string Country { get; set; }
        public string Course { get; set; }
    }
}
