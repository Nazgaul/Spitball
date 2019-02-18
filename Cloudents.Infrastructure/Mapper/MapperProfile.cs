using System.Collections.Generic;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.Auth;
using Cloudents.Infrastructure.Converters;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Search.Places;
using Cloudents.Infrastructure.Suggest;
using SearchResult = Cloudents.Core.DTOs.SearchResult;

namespace Cloudents.Infrastructure.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<GoogleGeoCodeDto, (Address address, GeoPoint point)>().ConvertUsing<GoogleGeoConverter>();
            CreateMap<BingWebPage, SearchResult>().ConvertUsing<BingConverter>();
            CreateMap<BingSuggest.SuggestionsObject, IEnumerable<string>>().ConvertUsing<BingSuggestConverter>();
        }
    }
}