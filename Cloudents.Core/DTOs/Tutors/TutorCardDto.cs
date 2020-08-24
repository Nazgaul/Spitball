using System.Collections.Generic;

namespace Cloudents.Core.DTOs.Tutors
{
    public class TutorCardDto //: FeedDto
    {
        public long UserId { get; set; }
        public string Name { get; set; } = null!;
        public string? Image { get; set; }
        public IEnumerable<string>? Courses { get; set; }

        public double? Rate { get; set; }
        public int ReviewsCount { get; set; }

        public string? Bio { get; set; }

        public int Lessons { get; set; }

       

    }
}
