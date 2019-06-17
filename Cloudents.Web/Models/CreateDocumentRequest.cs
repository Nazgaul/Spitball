using Cloudents.Core.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cloudents.Web.Extensions;
using Cloudents.Web.Resources;

namespace Cloudents.Web.Models
{
    public class CreateDocumentRequest : IValidatableObject
    {
        [Required]
        public string BlobName { get; set; }
        [Required]
        public string Name { get; set; }
       // [Required]
        public string Type { get; set; }

        [Required]
        [StringLength(Core.Entities.Course.MaxLength, ErrorMessage = "StringLength", MinimumLength = Core.Entities.Course.MinLength)]
        public string Course { get; set; }
        [CanBeNull]
        public string[] Tags { get; set; }

        public string Professor { get; set; }

        [Range(0, (int)Document.PriceLimit)]
        public decimal Price { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var stringLocalizer =
                validationContext.GetService(typeof(IStringLocalizer<DataAnnotationSharedResource>)) as
                    IStringLocalizer<DataAnnotationSharedResource>;
            if (Tags != null)
            {
                foreach (var tag in Tags)
                {
                    if (Tag.ValidateTag(tag)) continue;
                    var errorMessage = "Invalid length";
                    if (stringLocalizer != null)
                    {
                        errorMessage = stringLocalizer["TagCannotContain"];
                    }

                    yield return new ValidationResult(
                        errorMessage,
                        new[] {nameof(Tags)});
                }
            }

            if (string.IsNullOrEmpty(FriendlyUrlHelper.GetFriendlyTitle(Name)))
            {
                yield return new ValidationResult(
                    "File Name is invalid",
                    new[] { nameof(Name) });
            }
        }
    }
}
