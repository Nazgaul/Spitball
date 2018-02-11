using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;

namespace Cloudents.Infrastructure.Search.Job
{
    public class CareerJetConverter : ITypeConverter<CareerJetProvider.CareerJetResult, IEnumerable<JobDto>>
    {
        public IEnumerable<JobDto> Convert(CareerJetProvider.CareerJetResult source, IEnumerable<JobDto> destination, ResolutionContext context)
        {
            return source.jobs?.Select(s => new JobDto
            {
                DateTime = s.date,
                Url = s.url,
                Source = "CareerJet",
                Address = s.locations,
                Title = s.title,
                Company = s.company,
                CompensationType = "Paid",
                Responsibilities = s.description
            });
        }
    }
}