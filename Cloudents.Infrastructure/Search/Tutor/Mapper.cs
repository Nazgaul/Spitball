using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Search.Tutor
{
    [UsedImplicitly]
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Core.Entities.Search.Tutor, TutorDto>()
                .ForMember(m => m.Online, opt =>
                    opt.MapFrom(src => src.TutorFilter == TutorFilter.Online))

                .ForMember(m => m.PrioritySource, opt => opt.MapFrom(_ => PrioritySource.TutorWyzant));
            CreateMap<TutorMeSearch.Result, TutorDto>().ConvertUsing<TutorMeConverter>();
        }
    }
}
