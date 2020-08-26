using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class UpdateSettingsRequest
    {
        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "StringLength")]
        public string FirstName { get; set; }
        [StringLength(255, MinimumLength = 2, ErrorMessage = "StringLength")]
        [Required]
        public string LastName { get; set; }
        [StringLength(52, ErrorMessage = "StringLength")]
        public string? Title { get; set; }

        [StringLength(100, ErrorMessage = "StringLength")]
        public string? ShortParagraph { get; set; }

        public string? Paragraph { get; set; }

        public string Avatar { get; set; }
        public string Cover { get; set; }
    }
}
