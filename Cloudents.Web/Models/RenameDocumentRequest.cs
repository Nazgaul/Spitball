using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class RenameDocumentRequest
    {
        [Required]
        public long DocumentId { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "StringLength", MinimumLength = Core.Entities.Course.MinLength)]
        public string Name { get; set; }
    }
}
