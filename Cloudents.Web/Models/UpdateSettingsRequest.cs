using Cloudents.Core.Entities;
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
        [StringLength(25, MinimumLength = 0, ErrorMessage = "StringLength")]
        public string Title { get; set; }

        [StringLength(80, MinimumLength = 10, ErrorMessage = "StringLength")]
        public string ShortParagraph { get; set; }

        [StringLength(1000, MinimumLength = 15, ErrorMessage = "StringLength")]
        public string Paragraph { get; set; }
    }
}
