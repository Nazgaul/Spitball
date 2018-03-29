using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Search.Job
{
    [UsedImplicitly]
    public class Jobs2CareersConverter : ITypeConverter<Jobs2CareersProvider.Jobs2CareersResult, IEnumerable<JobDto>>
    {
        public IEnumerable<JobDto> Convert(Jobs2CareersProvider.Jobs2CareersResult source, IEnumerable<JobDto> destination, ResolutionContext context)
        {
            return source.Jobs.Select(s => new JobDto
            {
                DateTime = s.Date,
                Url = s.Url,
                Source = "Jobs2Careers",
                Address = s.City.FirstOrDefault(),
                Title = s.Title,
                Company = s.Company,
                CompensationType = "Paid",
                Responsibilities = RegEx.RemoveHtmlTags.Replace(s.Description, string.Empty)
            });
        }
    }
}