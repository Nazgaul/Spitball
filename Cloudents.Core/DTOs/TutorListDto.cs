using System.Collections.Generic;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;

namespace Cloudents.Core.DTOs
{
    public class TutorListDto
    {
       public long UserId { get; set; }
       public string Name { get; set; }
       public string Image { get; set; }
       public string Courses { get; set; }
       public decimal Price { get; set; }
       
       public float? Rate { get; set; }

       public string Bio { get; set; }

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
