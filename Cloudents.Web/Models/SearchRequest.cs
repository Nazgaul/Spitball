using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class SearchRequest
    {
        public string[] Source { get; set; }
        public long? University { get; set; }
        public long[] Course { get; set; }
        public string[] Term { get; set; }
        public int? Page { get; set; }
        public SearchCseRequestSort? Sort { get; set; }
    }
}
