using Cloudents.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateDocumentRequest
    {
        [Required]
        public string BlobName { get; set; }
        [Required]
        [StringLength(Document.MaxLength, ErrorMessage = "StringLength", MinimumLength = 4)]
        public string Name { get; set; }


        public bool Visible { get; set; }

    }
}
