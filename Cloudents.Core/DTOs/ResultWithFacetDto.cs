using System.Collections.Generic;
using JetBrains.Annotations;

namespace Cloudents.Core.DTOs
{
    public class ResultWithFacetDto<T>
    {
        [ItemCanBeNull]
        public IEnumerable<T> Result { get; set; }
        public IEnumerable<string> Facet { get; set; }
    }
}