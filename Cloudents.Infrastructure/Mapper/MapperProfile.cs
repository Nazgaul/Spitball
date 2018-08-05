using System.Collections.Generic;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Auth;
using Cloudents.Infrastructure.Converters;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Search.Places;
using Cloudents.Infrastructure.Suggest;
using Course = Cloudents.Core.Entities.Search.Course;
using SearchResult = Cloudents.Core.DTOs.SearchResult;
using University = Cloudents.Core.Entities.Search.University;

namespace Cloudents.Infrastructure.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<GoogleGeoCodeDto, (Address address, GeoPoint point)>().ConvertUsing<GoogleGeoConverter>();
            CreateMap<BingWebPage, SearchResult>().ConvertUsing<BingConverter>();
            CreateMap<BingSuggest.SuggestionsObject, IEnumerable<string>>().ConvertUsing<BingSuggestConverter>();

            CreateMap<Course, CourseDto>();
            CreateMap<University, UniversityDto>();
            //CreateMap<Question, QuestionDto>()
            //    .ForMember(m => m.Id, c => c.MapFrom(m => long.Parse(m.Id)))
            //    .ForMember(m => m.Price, c => c.MapFrom(m => decimal.Parse(m.Price)));

            CreateMap<GoogleToken, AuthDto>().ConvertUsing(o => new AuthDto
            {
                Name = o.Name,
                Email = o.Email
            });
        }
    }
}