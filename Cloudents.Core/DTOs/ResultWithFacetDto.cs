using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    //public class JobFacetDto
    //{
    //    public IEnumerable<JobDto> Jobs { get; set; }
    //    public IEnumerable<string> Facet { get; set; }
    //}

    public class ResultWithFacetDto<T>
    {
        public IEnumerable<T> Result { get; set; }
        public IEnumerable<string> Facet { get; set; }
    }
}