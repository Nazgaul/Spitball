using Cloudents.Core.Enum;

namespace Cloudents.Core.Query.Feed
{
    public class GetFeedQuery
    {
        public GetFeedQuery(long userId, int page, FeedType? filter, string country)
        {
            UserId = userId;
            Page = page;
            Filter = filter ?? FeedType.All;
            Country = country;
        }
        public long UserId { get; }
        public int Page { get; }
        public FeedType Filter { get; }
        public string Country { get; }
    }

    public class GetFeedWithCourseQuery : GetFeedQuery
    {
        public GetFeedWithCourseQuery(long userId, int page, FeedType? filter, string country, string course) : base(
             userId,  page, filter,  country)
        {
          
            Course = course;
        }

        public static GetFeedWithCourseQuery FromBase(GetFeedQuery query)
        {
            return new GetFeedWithCourseQuery(query.UserId,query.Page,query.Filter,query.Country,null);
        }
        public string Course { get; }
    }
}

