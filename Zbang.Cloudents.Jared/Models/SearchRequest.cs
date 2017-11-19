using Cloudents.Core.Enum;

namespace Zbang.Cloudents.Jared.Models
{

    public class SearchRequest
    {
        public string Source { get; set; }
        public long? University { get; set; }
        public string Course { get; set; }
        public string[] Query { get; set; }
        public int? Page { get; set; }
        public SearchCseRequestSort? Sort { get; set; }
    }
}