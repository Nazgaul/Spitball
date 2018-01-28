using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Converters;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Tutor, TutorDto>();

            CreateMap<string, Location>().ConvertUsing<IpConverter>();
            CreateMap<IpDto, Location>();

            CreateMap<DocumentSearchResult<Job>, ResultWithFacetDto<JobDto>>()
                .ConvertUsing<JobResultConverter>();

            CreateMap<Job, JobDto>().ConvertUsing(jo => new JobDto
            {
                Url = jo.Url,
                CompensationType = jo.Compensation,
                Company = jo.Company,
                DateTime = jo.DateTime,
                City = jo.City,
                Title = jo.Title,
                State = jo.State,
                Responsibilities = jo.Description,
                Source = jo.Source

            });
            CreateMap<Search.Entities.Course, CourseDto>();
            CreateMap<Search.Entities.University, UniversityDto>();

            CreateMap<JObject, IEnumerable<BookSearchDto>>().ConvertUsing((jo, bookSearch, c) => jo["response"]["page"]["books"]?["book"]?.Select(json => c.Mapper.Map<JToken, BookSearchDto>(json)));
            CreateMap<JToken, BookSearchDto>().ConvertUsing(jo => new BookSearchDto
            {
                Image = jo["image"]?["image"].Value<string>(),
                Author = jo["author"].Value<string>(),
                Binding = jo["binding"].Value<string>(),
                Edition = jo["edition"].Value<string>(),
                Isbn10 = jo["isbn10"].Value<string>(),
                Isbn13 = jo["isbn13"].Value<string>(),
                Title = jo["title"].Value<string>()

            });
            CreateMap<JObject, BookDetailsDto>().ConvertUsing<BookDetailConverter>();
            CreateMap<JObject, (string, IEnumerable<PlaceDto>)>().ConvertUsing<PlacesConverter>();
            CreateMap<JObject, PlaceDto>().ConvertUsing<PlaceConverter>();
            CreateMap<JObject, IEnumerable<TutorDto>>().ConvertUsing<TutorMeConverter>();
            //TODO
            CreateMap<JObject, Location>().ConvertUsing(jo =>
            {
                if (!string.Equals(jo["status"].Value<string>(), "ok", StringComparison.InvariantCultureIgnoreCase))
                    return null;
                var geo = jo["results"][0]["geometry"]["location"];
                return new Location
                {
                    Latitude = geo["lat"].Value<double>(),
                    Longitude = geo["lng"].Value<double>()
                };

            });
            CreateMap<JObject, AddressDto>().ConvertUsing(jo =>
            {
                if (!string.Equals(jo["status"].Value<string>(), "ok", StringComparison.InvariantCultureIgnoreCase))
                    return null;
                var geo = jo["results"][0]["formatted_address"].Value<string>();
                return new AddressDto(geo);
            });
        }
    }
}