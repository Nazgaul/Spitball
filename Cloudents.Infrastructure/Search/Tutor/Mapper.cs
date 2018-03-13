using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;

namespace Cloudents.Infrastructure.Search.Tutor
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Core.Entities.Search.Tutor, TutorDto>()
                .ForMember(m => m.Online, opt => 
                    opt.MapFrom(src => src.TutorFilter == TutorFilter.Online));
            CreateMap<TutorMeSearch.Result, TutorDto>().ConvertUsing<TutorMeConverter>();
        }
    }
}
