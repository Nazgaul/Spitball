using Cloudents.Core.DTOs;
using AutoMapper;

namespace Cloudents.Infrastructure.Search.Tutor
{
    public class TutorMeConverter : ITypeConverter<TutorMeSearch.Result, TutorDto>
    {
        public TutorDto Convert(TutorMeSearch.Result source, TutorDto destination, ResolutionContext context)
        {
            return new TutorDto
            {
                Description = source.About,
                Fee = 60,
                Image = source.Avatar.X300,
                Name = source.ShortName,
                Online = source.IsOnline,
                Source = "TutorMe",
                Url = $"https://tutorme.com/tutors/{source.Id}/"
            };
        }
    }
}
