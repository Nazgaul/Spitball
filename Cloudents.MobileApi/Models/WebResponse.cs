using System.Collections.Generic;
using Cloudents.Core.DTOs;

namespace Cloudents.Api.Models
{
    public class WebResponseWithFacet<T>
    {
        public IEnumerable<T> Result { get; set; }
        public IEnumerable<string> Facet { get; set; }

        public string NextPageLink { get; set; }
    }
}