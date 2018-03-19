using System.Collections.Generic;
using Cloudents.Core.DTOs;

namespace Cloudents.Api.Models
{
    public class WebResponse
    {
        public IEnumerable<SearchResult> Result { get; set; }
        public IEnumerable<string> Facet { get; set; }

        public string NextPageLink { get; set; }
    }
}