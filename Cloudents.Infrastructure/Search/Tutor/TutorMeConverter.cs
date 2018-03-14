using Cloudents.Core.DTOs;
using AutoMapper;
using Cloudents.Core;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Search.Tutor
{
    [UsedImplicitly]
    public class TutorMeConverter : ITypeConverter<TutorMeSearch.Result, TutorDto>
    {
        private const string Source = "TutorMe";

        public TutorDto Convert(TutorMeSearch.Result source, TutorDto destination, 
            ResolutionContext context)
        {
            return new TutorDto
            {
                Description = source.About,
                Fee = 60,
                Image = source.Avatar.X300,
                Name = source.ShortName,
                Online = source.IsOnline,
                //Source = Source,
                PrioritySource = PrioritySource.TutorMe,
                Url = $"https://tutorme.com/tutors/{source.Id}/"
            };
        }
    }
}
