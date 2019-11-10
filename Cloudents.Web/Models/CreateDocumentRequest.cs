using Cloudents.Core.Entities;
using Cloudents.Web.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateDocumentRequest : IValidatableObject
    {
        [Required]
        public string BlobName { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(Core.Entities.Course.MaxLength, ErrorMessage = "StringLength", MinimumLength = Core.Entities.Course.MinLength)]
        public string Course { get; set; }


        [Range(0, (int)Document.PriceLimit)]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(FriendlyUrlHelper.GetFriendlyTitle(Name)))
            {
                yield return new ValidationResult(
                    "File Name is invalid",
                    new[] { nameof(Name) });
            }
        }
    }
}
