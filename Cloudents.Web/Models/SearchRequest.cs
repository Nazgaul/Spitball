using Cloudents.Core.Enum;
using Cloudents.Web.Framework;

namespace Cloudents.Web.Models
{
    public class DocumentRequestAggregate : IPaging
    {
        public int Page { get; set; }
        public FeedType? Filter { get; set; }
    }


    public class DocumentRequestCourse : DocumentRequestAggregate
    {
        [RequiredPropertyForQuery]
        public string Course { get; set; }
    }

    public class DocumentRequestSearch : DocumentRequestAggregate
    {
        [RequiredPropertyForQuery]
        public string Term { get; set; }

    }

    public class DocumentRequestSearchCourse : DocumentRequestSearch
    {
        [RequiredPropertyForQuery]
        public string Course { get; set; }
    }
}