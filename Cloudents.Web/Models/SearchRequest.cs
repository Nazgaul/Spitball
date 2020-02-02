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


    //public abstract class VerticalRequest : IPaging
    //{
    //    /// <summary>
    //    /// User courses id
    //    /// </summary>
    //    public string Course { get; set; }

    //    [FromQuery(Name = "Uni")]
    //    public string University { get; set; }

    //    [IgnoreNextPageLink]
    //    public bool NeedUniversity => !string.IsNullOrEmpty(University);
    //    /// <inheritdoc />
    //    /// <summary>
    //    /// Page for paging
    //    /// </summary>
    //    public int Page { get; set; }

    //    /// <summary>
    //    /// The term of search
    //    /// </summary>
    //    public string Term { get; set; }


    //}
}