using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Infrastructure.Search.Job;

namespace Cloudents.Infrastructure.Converters
{
    public class ZipRecruiterConverter: ITypeConverter<ZipRecruiterClient.ZipRecruiterResult, IEnumerable<JobDto>>
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
                Source = "ZipRecruiter",
                Company = s.HiringCompany.Name,
                Address = s.Location,
                Title = s.Name,
                CompensationType = "paid",
                Responsibilities = RegEx.RemoveHtmlTags.Replace(s.Snippet,string.Empty)
            });
            //return new ResultWithFacetDto<JobDto>
            //{
            //    Result = source.Jobs.Select(s=> new JobDto
            //    {
            //        DateTime = s.PostedTime,
            //        Url = s.Url,
            //        Source = "ZipRecruiter",
            //        Company = s.HiringCompany.Name,
            //        Address = s.Location,
            //        Title = s.Name,
            //        CompensationType = "paid",
            //        Responsibilities = s.Snippet
            //    })
            //};
        }
    }
}