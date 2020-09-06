using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cloudents.Web.Models
{
    public class CreateStudyRoomRequest : IValidatableObject
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public IEnumerable<long> UserId { get; set; }

        [Required]
        [Range(0, 10000000)]
        public decimal Price { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           
            if ( UserId.Any() == false)
            {
                yield return new ValidationResult(
                    "Need to enter or users",
                    new[] { nameof(Name) });
            }
        }

        
    }

    public class CreateTailorEdStudyRoomRequest
    {
        public string Name { get; set; }

        [Range(1,50)]
        public int AmountOfUsers { get; set; }
    }
}