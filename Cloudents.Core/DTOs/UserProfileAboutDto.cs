using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class UserProfileAboutDto
    {
        public IEnumerable<CourseDto> Courses { get; set; }

        public string Bio { get; set; }

        public IEnumerable<string> Subjects { get; set; }

        public IEnumerable<TutorReviewDto> Reviews { get; set; }
    }
}