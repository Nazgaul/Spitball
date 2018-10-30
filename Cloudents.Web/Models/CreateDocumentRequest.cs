using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;

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
        public string[] Courses { get; set; }
        public string[] Tags { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            foreach (var tag in Tags)
            {
                if (tag.Length > Tag.MaxLength || tag.Length < Tag.MinLength)
                {
                    //TODO : v7 localize
                    yield return new ValidationResult(
                        "invalid length",
                        new[] { nameof(Tags) });
                }
            }


            foreach (var course in Courses)
            {
                if (course.Length > Course.MaxLength || course.Length < Course.MinLength)
                {
                    //TODO : v7 localize
                    yield return new ValidationResult(
                        "invalid length",
                        new[] { nameof(Courses) });
                }
            }

        }
    }
}
