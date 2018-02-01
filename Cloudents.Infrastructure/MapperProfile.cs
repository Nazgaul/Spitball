using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Converters;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Search.Job;
using Cloudents.Infrastructure.Search.Places;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json.Linq;
using SearchResult = Cloudents.Core.DTOs.SearchResult;

namespace Cloudents.Infrastructure
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Tutor, TutorDto>().ForMember(m => m.Online, opt => opt.MapFrom(src => src.TutorFilter == TutorFilter.Online));
            CreateMap<string, Location>().ConvertUsing<IpConverter>();
            CreateMap<IpDto, Location>().ForMember(f => f.Point, opt => opt.MapFrom(src => new GeoPoint { Latitude = src.Latitude, Longitude = src.Longitude }));
            CreateMap<GoogleGeoCodeDto, Location>().ConvertUsing<GoogleGeoConverter>();
            CreateMap<BingSearch.WebPage, SearchResult>().ConvertUsing<BingConverter>();
            //ZipRecruiterConverter

            CreateMap<DocumentSearchResult<Job>, ResultWithFacetDto<JobDto>>()
                .ConvertUsing<JobResultConverter>();

            CreateMap<ZipRecruiterClient.ZipRecruiterResult, IEnumerable<JobDto>>()
                .ConvertUsing<ZipRecruiterConverter>();

            CreateMap<Job, JobDto>().ConvertUsing(jo => new JobDto
            {
                Url = jo.Url,
                CompensationType = jo.Compensation,
                Company = jo.Company,
                DateTime = jo.DateTime.GetValueOrDefault(),
                Address = $"{jo.City}, {jo.State}",
                Title = jo.Title,
                Responsibilities = jo.Description,
                Source = jo.Source
            });
            CreateMap<Search.Entities.Course, CourseDto>();
            CreateMap<Search.Entities.University, UniversityDto>();

            CreateMap<BookSearch.BookDetailResult, IEnumerable<BookSearchDto>>()
                .ConvertUsing((jo, _, c) => jo.Response.Page.Books?.Book.Select(json => c.Mapper.Map<BookSearch.BookDetail, BookSearchDto>(json)));
            CreateMap<BookSearch.BookDetail, BookSearchDto>().ConvertUsing(jo => new BookSearchDto
            {
                Image = jo.Image?.Image,
                Author = jo.Author,
                Binding = jo.Binding,
                Edition = jo.Edition,
                Isbn10 = jo.Isbn10,
                Isbn13 = jo.Isbn13,
                Title = jo.Title

            });
            CreateMap<BookSearch.BookDetailResult, BookDetailsDto>().ConvertUsing<BookDetailConverter>();
            CreateMap<JObject, (string, IEnumerable<PlaceDto>)>().ConvertUsing<PlacesConverter>();
            CreateMap<JObject, PlaceDto>().ConvertUsing<PlaceConverter>();
            //CreateMap<JObject, IEnumerable<TutorDto>>().ConvertUsing<TutorMeConverter>();
        }
    }
}