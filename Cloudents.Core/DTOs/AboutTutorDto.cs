using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class AboutTutorDto
    {
        [EntityBind(nameof(BaseUser.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(TutorReview.Review))]
        public string Review { get; set; }
        [EntityBind(nameof(TutorReview.Rate))]
        public decimal Rate { get; set; }
        [EntityBind(nameof(BaseUser.Image))]
        public string Image { get; set; }
    }
}
