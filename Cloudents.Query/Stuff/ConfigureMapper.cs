using AutoMapper;
using Cloudents.Core.DTOs;

namespace Cloudents.Query.Stuff
{
    public class ConfigureMapper : Profile
    {
        public ConfigureMapper()
        {
            CreateMap<UserPurchasedDocumentsQueryResult, DocumentFeedDto>()
                .ForMember(m => m.University, x => x.MapFrom(z => z.UniversityName))
                .ForMember(m => m.Course, x => x.MapFrom(z => z.CourseName))
                .ForMember(m => m.Snippet, x => x.MapFrom(z => z.MetaContent))
                .ForMember(m => m.Title, x => x.MapFrom(z => z.Name))
                .ForMember(m => m.TypeStr, x => x.MapFrom(z => z.Type))
                //.ForMember(m => m.User, x => x.MapFrom(z => z.Type))
                ;
            //CreateMap<GoogleGeoCodeDto, (Address address, GeoPoint point)>().ConvertUsing<GoogleGeoConverter>();
            //CreateMap<BingWebPage, SearchResult>().ConvertUsing<BingConverter>();
            //CreateMap<BingSuggest.SuggestionsObject, IEnumerable<string>>().ConvertUsing<BingSuggestConverter>();

            //CreateMap<GoogleToken, ExternalAuthDto>().ConvertUsing(o => new ExternalAuthDto
            //{
            //    Name = o.Name,
            //    Email = o.Email,
            //    Id = o.Sub
            //});
        }
    }
}