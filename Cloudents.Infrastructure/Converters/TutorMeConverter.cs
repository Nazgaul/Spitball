using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure.Converters
{
    public class TutorMeConverter : ITypeConverter<JObject, IEnumerable<TutorDto>>
    {
        public IEnumerable<TutorDto> Convert(JObject source, IEnumerable<TutorDto> destination, ResolutionContext context)
        {
            return source["results"].Children().Select(result => new TutorDto
            {
                Url = $"https://tutorme.com/tutors/{result["id"].Value<string>()}",
                Image = result["avatar"]["x300"].Value<string>(),
                Name = result["shortName"].Value<string>(),
                Online = result["isOnline"].Value<bool>(),
                Description = result["about"].Value<string>(),
                TermCount = result.ToString().Split(new[] { context.Items["term"].ToString() },
                    StringSplitOptions.RemoveEmptyEntries).Length
            });
        }
    }
}
