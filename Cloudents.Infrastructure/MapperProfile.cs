using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
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
            CreateMap<Search.Entities.Tutor, TutorDto>()
                .ForMember(d => d.Location, o => o.ResolveUsing(p => new GeoPoint
                {
                    Latitude = p.Location.Latitude,
                    Longitude = p.Location.Longitude
                }))
                .ForMember(d => d.TermCount, o => o.ResolveUsing((t, ts, i, c) =>
                {
                    var temp = $"{t.City} {t.State} {string.Join(" ", t.Subjects)} {string.Join(" ", t.Extra)}";
                    return temp.Split(new[] { c.Items["term"].ToString() },
                        StringSplitOptions.RemoveEmptyEntries).Length;
                }));

            CreateMap<DocumentSearchResult<Search.Entities.Job>, ResultWithFacetDto<JobDto>>()
                .ConvertUsing<JobResultConverter>();

            CreateMap<Search.Entities.Job, JobDto>();
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
            CreateMap<JObject, GeoPoint>().ConvertUsing(jo =>
            {
                var geo = jo["results"][0]["geometry"]["location"];
                return new GeoPoint
                {
                    Latitude = geo["lat"].Value<double>(),
                    Longitude = geo["lng"].Value<double>()
                };
            });
            CreateMap<string, IpDto>().ConvertUsing<IpConverter>();
        }
    }
}