using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class ResultWithFacetDto<T>
    {
        public IEnumerable<T> Result { get; set; }
        public IEnumerable<string> Facet { get; set; }
    }
}