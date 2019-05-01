using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class TutorListDto
    {
       [EntityBind(nameof(RegularUser.Id))]
       public long UserId { get; set; }
       [EntityBind(nameof(RegularUser.Name))]
       public string Name { get; set; }
       [EntityBind(nameof(RegularUser.Image))]
       public string Image { get; set; }
       [EntityBind(nameof(Course.Id))]
       public string Courses { get; set; }
       [EntityBind(nameof(Tutor.Price))]
       public decimal Price { get; set; }
       
       [EntityBind(nameof(TutorReview.Rate))]
       public float? Rate { get; set; }

       [EntityBind(nameof(Tutor.Bio))]
       public string Bio { get; set; }

       [EntityBind(nameof(TutorReview))]
       public int ReviewsCount { get; set; }
    }
}
