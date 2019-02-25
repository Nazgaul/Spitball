//using System.Linq;
//using AutoMapper;
//using Cloudents.Core;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Extension;
//using JetBrains.Annotations;

//namespace Cloudents.Infrastructure.Search.Job
//{
//    [UsedImplicitly]
//    public class Mapper : Profile
//    {
//        public Mapper()
//        {
          

//          //  CreateMap<JobProviderDto, JobDto>();

          

//            //CreateMap<Jobs2CareersProvider.Job, JobProviderDto>()
//            //    .ConvertUsing(s=> new JobProviderDto
//            //    {
//            //        DateTime = s.Date,
//            //        Url = s.Url,
//            //        PrioritySource = PrioritySource.JobJobs2Careers,
//            //        Address = s.City.FirstOrDefault(),
//            //        Title = s.Title,
//            //        Company = s.Company,
//            //        CompensationType = "Paid",
//            //        Responsibilities = s.Description.StripAndDecode()
//            //    });

//            //CreateMap<IndeedProvider.Result, JobProviderDto>()
//            //    .ConvertUsing(s => new JobProviderDto
//            //    {
//            //        DateTime = s.Date,
//            //        Url = s.Url,
//            //        PrioritySource = PrioritySource.JobIndeed,
//            //        //Source = "Indeed",
//            //        Address = s.FormattedLocation,
//            //        Title = s.JobTitle,
//            //        Company = s.Company,
//            //        CompensationType = "Paid",
//            //        Responsibilities = s.Snippet,
//            //    });

//            //CreateMap<ZipRecruiterClient.Job, JobProviderDto>().ConvertUsing(s => new JobProviderDto
//            //{
//            //    DateTime = s.PostedTime,
//            //    Url = s.Url,
//            //    PrioritySource = PrioritySource.JobZipRecruiter,
//            //    //Source = "ZipRecruiter",
//            //    Company = s.HiringCompany.Name,
//            //    Address = s.Location,
//            //    Title = s.Name,
//            //    CompensationType = "Paid",
//            //    Responsibilities = s.Snippet.StripAndDecode()
//            //});

//            CreateMap<CareerJetProvider.Job, JobProviderDto>().ConvertUsing(s => new JobProviderDto
//            {
//                DateTime = s.Date,
//                Url = s.Url,
//                PrioritySource = PrioritySource.JobCareerJet,
//                Address = s.Locations,
//                Title = s.Title,
//                Company = s.Company,
//                CompensationType = "Paid",
//                Responsibilities = s.Description.StripAndDecode()
//            });
//        }
//    }
//}
