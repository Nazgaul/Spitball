using System.Collections.Generic;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Infrastructure.Converters;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Search.Job
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<DocumentSearchResult<Core.Entities.Search.Job>, ResultWithFacetDto<JobDto>>()
                .ConvertUsing<JobResultConverter>();

            CreateMap<ZipRecruiterClient.ZipRecruiterResult, IEnumerable<JobDto>>()
                .ConvertUsing<ZipRecruiterConverter>();

            CreateMap<Jobs2CareersProvider.Jobs2CareersResult, IEnumerable<JobDto>>()
                .ConvertUsing<Jobs2CareersConverter>();

            CreateMap<IndeedProvider.IndeedResult, IEnumerable<JobDto>>()
                .ConvertUsing<IndeedConverter>();
            CreateMap<CareerJetProvider.CareerJetResult, IEnumerable<JobDto>>()
                .ConvertUsing<CareerJetConverter>();

            CreateMap<Core.Entities.Search.Job, JobDto>().ConvertUsing(jo => new JobDto
            {
                Url = jo.Url,
                CompensationType = jo.Compensation,
                Company = jo.Company,
                DateTime = jo.DateTime.GetValueOrDefault(),
                Address = $"{jo.City}, {jo.State}",
                Title = jo.Title,
                Responsibilities = jo.Description,
                PrioritySource = PrioritySource.JobWayUp
                //Source = jo.Source,
            });
        }
    }
}
