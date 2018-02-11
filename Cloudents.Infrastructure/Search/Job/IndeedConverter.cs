using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;

namespace Cloudents.Infrastructure.Search.Job
{
    public class IndeedConverter : ITypeConverter<IndeedProvider.IndeedResult, IEnumerable<JobDto>>
    {
        public IEnumerable<JobDto> Convert(IndeedProvider.IndeedResult source, IEnumerable<JobDto> destination, ResolutionContext context)
        {
            return source.results.Select(s => new JobDto
            {
                DateTime = s.date,
                Url = s.url,
                Source = "Indeed",
                Address = s.formattedLocation,
                Title = s.jobtitle,
                Company = s.company,
                CompensationType = "Paid",
                Responsibilities = s.snippet
            });
        }
    }
}