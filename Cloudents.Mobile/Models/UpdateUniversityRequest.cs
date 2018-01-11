using System.ComponentModel.DataAnnotations;

namespace Cloudents.Mobile.Models
{
    public class UpdateUniversityRequest
    {
        [Required]
        public long UniversityId { get; set; }
    }
}