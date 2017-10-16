using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class JobFacetDto
    {
        public IEnumerable<JobDto> Jobs { get; set; }
        public IEnumerable<string> Types { get; set; }
    }
}