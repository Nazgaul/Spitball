using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class SetSubjectToCourseRequest
    {
        [Required]
        public string CourseName { get; set; }

        [Required]
        public string Subject { get; set; }
    }
}
