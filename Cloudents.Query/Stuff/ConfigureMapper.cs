using AutoMapper;
using Cloudents.Core.DTOs;
using System.Collections.Generic;

namespace Cloudents.Query.Stuff
{
    public class ConfigureMapper : Profile
    {
        public ConfigureMapper()
        {
         

            CreateMap<(long, IEnumerable<LeaderBoardDto>), LeaderBoardQueryResult>()
                
                .ForMember(m => m.LeaderBoard, x => x.MapFrom(z => z.Item2))
                .ForMember(m => m.SBL, x => x.MapFrom(z => z.Item1)).ReverseMap();

            CreateMap<DocumentFeedDto, UserPurchasedDocumentsQueryResult>()
                .ForMember(m => m.University, x => x.MapFrom(z => z.University))
                .ForMember(m => m.Course, x => x.MapFrom(z => z.Course))
                .ForMember(m => m.Snippet, x => x.MapFrom(z => z.Snippet))
                .ForMember(m => m.Title, x => x.MapFrom(z => z.Title))
                .ForMember(m => m.TypeStr, x => x.MapFrom(z => z.TypeStr))
                .ForMember(m => m.UserId, x => x.MapFrom(z => z.User.Id))
                .ForMember(m => m.UserName, x => x.MapFrom(z => z.User.Name))
                .ForMember(m => m.UserScore, x => x.MapFrom(z => z.User.Score))
                .ForMember(m => m.Id, x => x.MapFrom(z => z.Id))
                .ForMember(m => m.DateTime, x => x.MapFrom(z => z.DateTime))
                .ForMember(m => m.Professor, x => x.MapFrom(z => z.Professor))
                .ForMember(m => m.Views, x => x.MapFrom(z => z.Views))
                .ForMember(m => m.Price, x => x.MapFrom(z => z.Price))
                .ForMember(m => m.Downloads, x => x.MapFrom(z => z.Downloads))
                .ForMember(m => m.VoteCount, x => x.MapFrom(z => z.Vote.Votes))
                .ForMember(m => m.Source, x => x.MapFrom(z => z.Source))
                .ReverseMap()
                .ForPath(s => s.User.Id, opt => opt.MapFrom(src => src.UserId))
                .ForPath(s => s.User.Name, opt => opt.MapFrom(src => src.UserName))
                .ForPath(s => s.User.Score, opt => opt.MapFrom(src => src.UserScore))
                .ForPath(s => s.Url, opt => opt.Ignore());
           

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