using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateCourse
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required]
        public long? University { get; set; }
    }
}
