﻿using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Cloudents.Web.Models
{

    public class CreateDocumentResponse
    {
        public CreateDocumentResponse(string url)
        {
            Url = url;
        }

        public string Url { get; set; }
    }
    public class CreateDocumentRequest : IValidatableObject
    {
        [Required]
        public string BlobName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DocumentType Type { get; set; }

        [Required]
        [StringLength(Core.Entities.Db.Course.MaxLength,ErrorMessage = "StringLength", MinimumLength = Core.Entities.Db.Course.MinLength)]
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
