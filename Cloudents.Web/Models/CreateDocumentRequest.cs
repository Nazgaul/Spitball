using System.Collections.Generic;
using Cloudents.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateDocumentRequest : IValidatableObject
    {
        public string? BlobName { get; set; }
        [Required]
        [StringLength(Document.MaxLength, ErrorMessage = "StringLength", MinimumLength = 4)]
        public string Name { get; set; }


        public bool Visible { get; set; }

        public long? Id { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Id == null && BlobName == null)
            {
                yield return new ValidationResult("Should have blob or id");
            }
        }
    }
}
