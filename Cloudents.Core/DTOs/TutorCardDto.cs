using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class TutorCardDto
    {
       public long UserId { get; set; }
       public string Name { get; set; }
       public string Image { get; set; }
       public IEnumerable<string> Courses { get; set; } 
       public IEnumerable<string> Subjects { get; set; }
       public decimal Price { get; set; }
       
       public double? Rate { get; set; }
       public int ReviewsCount { get; set; }

        public string Bio { get; set; }

        public string University { get; set; }

        public int Lessons { get; set; }

        private sealed class UserIdEqualityComparer : IEqualityComparer<TutorCardDto>
        {
            public bool Equals(TutorCardDto x, TutorCardDto y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.UserId == y.UserId;
            }

            public int GetHashCode(TutorCardDto obj)
            {
                return obj.UserId.GetHashCode();
            }
        }

        public static IEqualityComparer<TutorCardDto> UserIdComparer { get; } = new UserIdEqualityComparer();
    }
}
