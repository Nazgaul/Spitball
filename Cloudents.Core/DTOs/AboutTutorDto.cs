using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class AboutTutorDto
    {
        [EntityBind(nameof(BaseUser.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(BaseUser.Name))]
        public string Name { get; set; }
        [EntityBind(nameof(TutorReview.Review))]
        public string Review { get; set; }
        [EntityBind(nameof(TutorReview.Rate))]
        public decimal Rate { get; set; }
        [EntityBind(nameof(BaseUser.ImageName))]
        public string Image { get; set; }
    }
}
