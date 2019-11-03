using Cloudents.Core.Enum;

namespace Cloudents.Core.Query.Feed
{
    public class GetFeedQuery
    {
        public GetFeedQuery(long userId, int page, FeedType? filter, string country, string course)
        {
            UserId = userId;
            Page = page;
            Filter = filter;
            Country = country;
            Course = course;
        }
        public long UserId { get; }
        public int Page { get; }
        public FeedType? Filter { get; }
        public string Country { get; }
        public string Course { get; }
    }
}

