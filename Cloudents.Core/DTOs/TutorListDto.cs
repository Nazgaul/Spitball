using System.Collections.Generic;
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

       private sealed class UserIdEqualityComparer : IEqualityComparer<TutorListDto>
       {
           public bool Equals(TutorListDto x, TutorListDto y)
           {
               if (ReferenceEquals(x, y)) return true;
               if (ReferenceEquals(x, null)) return false;
               if (ReferenceEquals(y, null)) return false;
               if (x.GetType() != y.GetType()) return false;
               return x.UserId == y.UserId;
           }

           public int GetHashCode(TutorListDto obj)
           {
               return obj.UserId.GetHashCode();
           }
       }

       public static IEqualityComparer<TutorListDto> UserIdComparer { get; } = new UserIdEqualityComparer();
    }
}
