using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Search.Job
{
    [UsedImplicitly]
    public class IndeedConverter : ITypeConverter<IndeedProvider.IndeedResult, IEnumerable<JobDto>>
    {
        public IEnumerable<JobDto> Convert(IndeedProvider.IndeedResult source, IEnumerable<JobDto> destination, ResolutionContext context)
        {
            return source.Results.Select(s => new JobDto
            {
                DateTime = s.Date,
                Url = s.Url,
                PrioritySource = PrioritySource.JobIndeed,
                //Source = "Indeed",
                Address = s.FormattedLocation,
                Title = s.JobTitle,
                Company = s.Company,
                CompensationType = "Paid",
                Responsibilities = s.Snippet,
            });
        }
    }
}