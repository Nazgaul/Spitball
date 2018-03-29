﻿using System.Collections.Generic;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Converters;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Search.Places;
using Cloudents.Infrastructure.Suggest;
using SearchResult = Cloudents.Core.DTOs.SearchResult;

namespace Cloudents.Infrastructure
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<GoogleGeoCodeDto, (Address address, GeoPoint point)>().ConvertUsing<GoogleGeoConverter>();
            CreateMap<BingSearch.WebPage, SearchResult>().ConvertUsing<BingConverter>();
            CreateMap<BingSuggest.SuggestionsObject, IEnumerable<string>>().ConvertUsing<BingSuggestConverter>();

            CreateMap<Course, CourseDto>();
            CreateMap<University, UniversityDto>();

            //CreateMap<JObject, (string, IEnumerable<PlaceDto>)>().ConvertUsing<PlacesConverter>();
            //CreateMap<JObject, PlaceDto>().ConvertUsing<PlaceConverter>();
        }
    }
}