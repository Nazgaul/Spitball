using Cloudents.Domain.Entities;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cloudents.Common.Enum;
using Cloudents.Web.Resources;
using JetBrains.Annotations;

namespace Cloudents.Web.Models
{
    public class CreateDocumentRequest : IValidatableObject
    {
        [Required]
        public string BlobName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DocumentType Type { get; set; }

        [Required]
        [StringLength(Domain.Entities.Course.MaxLength,ErrorMessage = "StringLength", MinimumLength = Domain.Entities.Course.MinLength)]
        public string Course { get; set; }
        [CanBeNull]
        public string[] Tags { get; set; }

        public string Professor { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var stringLocalizer =
                validationContext.GetService(typeof(IStringLocalizer<DataAnnotationSharedResource>)) as
                    IStringLocalizer<DataAnnotationSharedResource>;
            if (Tags != null)
            {
                foreach (var tag in Tags)
                {
                    if (tag.Contains(","))
                    {
                        var errorMessage = "Invalid length";
                        if (stringLocalizer != null)
                        {
                            errorMessage = stringLocalizer["TagCannotContain"];
                        }

                        yield return new ValidationResult(
                            errorMessage,
                            new[] { nameof(Tags) });
                        
                    }
                    if (tag.Length > Tag.MaxLength || tag.Length < Tag.MinLength)
                    {
                      
                        var errorMessage = "Invalid length";
                        if (stringLocalizer != null)
                        {
                            errorMessage = stringLocalizer["StringLength"];
                        }

                        yield return new ValidationResult(
                            errorMessage,
                            new[] {nameof(Tags)});
                    }
                }
            }
        }
    }
}
