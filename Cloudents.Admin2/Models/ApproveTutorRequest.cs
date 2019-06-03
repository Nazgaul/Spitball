using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class ApproveTutorRequest
    {
        [Required]
        public long Id { get; set; }
    }
}
