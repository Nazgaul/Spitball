using Cloudents.Core.Entities;
using Cloudents.Web.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class CreateDocumentRequest : IValidatableObject
    {
        [Required]
        public string BlobName { get; set; }
        [Required]
        [StringLength(Document.MaxLength, ErrorMessage = "StringLength", MinimumLength = Core.Entities.Course.MinLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(Document.MaxLength, ErrorMessage = "StringLength", MinimumLength = Document.MinLength)]
        public string Course { get; set; }


        [Range(0, (int)Document.PriceLimit)]
        public decimal? Price { get; set; }

        public string? Description { get; set; }

        public PriceType PriceType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(FriendlyUrlHelper.GetFriendlyTitle(Name)))
            {
                yield return new ValidationResult(
                    "File Name is invalid",
                    new[] { nameof(Name) });
            }

            if (PriceType == PriceType.HasPrice && Price == null)
            {
                yield return new ValidationResult(
                    "Need to have price",
                    new[] { nameof(Price) });
            }
        }
    }
}
