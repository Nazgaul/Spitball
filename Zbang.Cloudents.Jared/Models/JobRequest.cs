using Cloudents.Core.Enum;
using Cloudents.Core.Models;

namespace Zbang.Cloudents.Jared.Models
{
    public class JobRequest
    {
        public string[] Term { get; set; }
        public JobRequestFilter? Filter { get; set; }
        public JobRequestSort? Sort { get; set; }
        public GeoPoint Location { get; set; }
        public string[] Facet { get; set; }

        public int Page { get; set; }
    }
}