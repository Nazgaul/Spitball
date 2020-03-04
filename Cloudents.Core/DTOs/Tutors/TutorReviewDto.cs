using System;

namespace Cloudents.Core.DTOs.Tutors
{
    public class TutorReviewDto
    {
        public string ReviewText { get; set; }
        public float Rate { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public long Id { get; set; }
    }
}
