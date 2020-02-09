using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class EditSubjectRequest : IValidatableObject
    {
        [Required]
        public long SubjectId { get; set; }

        [RegularExpression(@"[A-Za-z0-9|\s!@#$%^&*()_+-]*")]
        [StringLength(300)]
        public string EnSubjectName { get; set; }

        [RegularExpression(@"[א-ת0-9|\s!@#$%^&*()_+-]*")]
        [StringLength(150)]
        public string HeSubjectName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(EnSubjectName) && string.IsNullOrEmpty(HeSubjectName))
            {
                yield return new ValidationResult(
                                "Invalid Input");
            }
        }
    }
}
