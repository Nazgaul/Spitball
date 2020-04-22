using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class MigrateCourseRequest
    {
        [Required]
        public string CourseToRemove { get; set; }
        [Required]
        public string CourseToKeep { get; set; }
    }
}
