using AutoMapper;
using Cloudents.Core.DTOs;
using System.Collections.Generic;

namespace Cloudents.Query.Stuff
{
    public class ConfigureMapper : Profile
    {
        public ConfigureMapper()
        {
         

            CreateMap<(long, IEnumerable<LeaderBoardDto>), LeaderBoardResultDto>()
                
                .ForMember(m => m.LeaderBoard, x => x.MapFrom(z => z.Item2))
                .ForMember(m => m.SBL, x => x.MapFrom(z => z.Item1)).ReverseMap();

        }
    }
}