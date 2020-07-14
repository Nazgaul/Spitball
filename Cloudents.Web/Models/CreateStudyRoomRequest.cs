using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cloudents.Core.Enum;

namespace Cloudents.Web.Models
{
    public class CreateLiveStudyRoomRequest
    {
        [Required]
        public string Name { get; set; }


        [Required]
        [Range(0, 10000000)]
        public decimal Price { get; set; }

       
        [Required]
        public DateTime? Date { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            if (Date.GetValueOrDefault(DateTime.MaxValue) < DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Date should be in the future",
                    new[] { nameof(Name) });
            }
        }

        public string? Description { get; set; }
    }
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


    public class CreateStudyRoomResponse
    {
        public CreateStudyRoomResponse(Guid studyRoomId, string identifier)
        {
            StudyRoomId = studyRoomId;
            Identifier = identifier;
        }

        public Guid StudyRoomId { get; }

        public string Identifier { get; }

    }




}