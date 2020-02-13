using System.Collections.Generic;

namespace Cloudents.Core.DTOs.Users
{
    public class UserProfileAboutDto
    {
        // public IEnumerable<CourseDto> Courses { get; set; }

        // public string Bio { get; set; }

        public IEnumerable<TutorReviewDto> Reviews { get; set; }
        public IEnumerable<RatesDto> Rates { get; set; }
    }

    public class RatesDto
    {
        public int Rate { get; set; }
        public int Users { get; set; }
    }
}