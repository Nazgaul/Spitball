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
        [StringLength(28, MinimumLength = 0, ErrorMessage = "StringLength")]
        public string Title { get; set; }

        [StringLength(96, MinimumLength = 0, ErrorMessage = "StringLength")]
        public string ShortParagraph { get; set; }

        [StringLength(1000, MinimumLength = 0, ErrorMessage = "StringLength")]
        public string Paragraph { get; set; }
    }
}
