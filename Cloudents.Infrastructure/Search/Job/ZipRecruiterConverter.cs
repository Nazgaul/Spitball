using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.DTOs;

namespace Cloudents.Infrastructure.Search.Job
{
    public class ZipRecruiterConverter : ITypeConverter<ZipRecruiterClient.ZipRecruiterResult, IEnumerable<JobDto>>
    {
        public IEnumerable<JobDto> Convert(ZipRecruiterClient.ZipRecruiterResult source, IEnumerable<JobDto> destination, ResolutionContext context)
        {
            if (!source.Success)
            {
                return null;
            }

            return source.Jobs.Select(s => new JobDto
            {
                DateTime = s.PostedTime,
                Url = s.Url,
                PrioritySource = PrioritySource.JobZipRecruiter,
                Company = s.HiringCompany.Name,
                Address = s.Location,
                Title = s.Name,
                CompensationType = "Paid",
                Responsibilities = RegEx.RemoveHtmlTags.Replace(s.Snippet, string.Empty)
            });
        }
    }
}