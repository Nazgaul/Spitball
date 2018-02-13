using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.DTOs;

namespace Cloudents.Infrastructure.Search.Job
{
    public class CareerJetConverter : ITypeConverter<CareerJetProvider.CareerJetResult, IEnumerable<JobDto>>
    {
        public IEnumerable<JobDto> Convert(CareerJetProvider.CareerJetResult source, IEnumerable<JobDto> destination, ResolutionContext context)
        {
            return source.Jobs?.Select(s => new JobDto
            {
                DateTime = s.Date,
                Url = s.Url,
                Source = "CareerJet",
                Address = s.Locations,
                Title = s.Title,
                Company = s.Company,
                CompensationType = "Paid",
                Responsibilities = RegEx.RemoveHtmlTags.Replace(s.Description,string.Empty)
            });
        }
    }
}