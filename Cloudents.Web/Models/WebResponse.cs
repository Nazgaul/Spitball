using System.Collections.Generic;

namespace Cloudents.Web.Models
{
    public class WebResponseWithFacet<T>
    {
        public IEnumerable<T> Result { get; set; }
        public IEnumerable<string> Facet { get; set; }

        public string NextPageLink { get; set; }
    }
}