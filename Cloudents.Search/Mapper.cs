//using AutoMapper;
//using Cloudents.Core;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Enum;
//using Cloudents.Search.Job;
//using JetBrains.Annotations;
//using Microsoft.Azure.Search.Models;

//namespace Cloudents.Search
//{
//    [UsedImplicitly]
//    public class Mapper : Profile
//    {
//        public Mapper()
//        {
//            //CreateMap<Entities.Tutor, TutorDto>()
//            //    .ForMember(m => m.Online, opt =>
//            //        opt.MapFrom(src => src.TutorFilter == TutorFilter.Online))

//            //    .ForMember(m => m.PrioritySource, opt => opt.MapFrom(_ => PrioritySource.TutorWyzant));

//            CreateMap<DocumentSearchResult<Entities.Job>, ResultWithFacetDto<JobProviderDto>>()
//                .ConvertUsing<AzureJobSearchConverter>();

//            //CreateMap<Entities.Job, JobProviderDto>().ConvertUsing(jo => new JobProviderDto
//            //{
//            //    Url = jo.Url,
//            //    CompensationType = "Paid",
//            //    Company = jo.Company,
//            //    DateTime = jo.DateTime.GetValueOrDefault(),
//            //    Address = $"{jo.City}, {jo.State}",
//            //    Title = jo.Title,
//            //    Responsibilities = jo.Description,
//            //    PrioritySource = PrioritySource.JobWayUp,
//            //});
//        }
//    }
//}
