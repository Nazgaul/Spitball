using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RenameDocumentRequest
    {
        [Required]
        public long DocumentId { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "StringLength", MinimumLength = 4)]
        public string Name { get; set; }
    }
}
