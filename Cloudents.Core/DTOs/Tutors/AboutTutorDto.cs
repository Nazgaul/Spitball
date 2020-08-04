using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs.Tutors
{
    public class AboutTutorDto
    {
        [EntityBind(nameof(User.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(User.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(TutorReview.Review))]
        public string Review { get; set; }
        [EntityBind(nameof(TutorReview.Rate))]
        public decimal Rate { get; set; }
        [EntityBind(nameof(User.ImageName))]
        public string? Image { get; set; }
    }
}
