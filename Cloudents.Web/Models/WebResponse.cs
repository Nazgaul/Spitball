using System.Collections.Generic;

namespace Cloudents.Web.Models
{
    public class WebResponseWithFacet<T>
    {
        public IEnumerable<T> Result { get; set; }

        public IEnumerable<string> Filters { get; set; }

        public long? Count { get; set; }
        public string NextPageLink { get; set; }
    }
}