using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class CreateCourseRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public long Subject { get; set; }
    }
}