﻿//using System.Collections.Generic;
//using AutoMapper;
//using Cloudents.Core.Models;
//using Cloudents.Infrastructure.Converters;
//using Cloudents.Infrastructure.Search;
//using Cloudents.Infrastructure.Search.Places;
//using Cloudents.Infrastructure.Suggest;

//namespace Cloudents.Infrastructure.Mapper
//{
//    public class MapperProfile : Profile
//    {
//        public MapperProfile()
//        {
//            CreateMap<GoogleGeoCodeDto, (Address address, GeoPoint point)>().ConvertUsing<GoogleGeoConverter>();
//            //CreateMap<BingWebPage, SearchResult>().ConvertUsing<BingConverter>();
//            CreateMap<BingSuggest.SuggestionsObject, IEnumerable<string>>().ConvertUsing<BingSuggestConverter>();
//        }
//    }
//}