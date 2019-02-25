//using AutoMapper;
//using Cloudents.Core;
//using Cloudents.Core.DTOs;
//using JetBrains.Annotations;

//namespace Cloudents.Infrastructure.Search.Tutor
//{
//    [UsedImplicitly]
//    public class Mapper : Profile
//    {
//        public Mapper()
//        {
//            CreateMap<TutorMeSearch.Result, TutorDto>().ConvertUsing(source => new TutorDto
//            {
//                Description = source.About,
//                Fee = 60,
//                Image = source.Avatar.X300,
//                Name = source.ShortName,
//                Online = source.IsOnline,
//                //Source = "TutorMe",
//                PrioritySource = PrioritySource.TutorMe,
//                Url = $"https://tutorme.com/tutors/{source.Id}/"
//            });
//        }
//    }
//}
