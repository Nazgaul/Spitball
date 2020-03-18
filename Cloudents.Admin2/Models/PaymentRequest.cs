using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class PaymentRequest : IValidatableObject
    {
        public decimal StudentPay { get; set; }
        //[Range(5, 50000)]
        public decimal SpitballPay { get; set; }

        public long UserId { get; set; }
        public long TutorId { get; set; }

        public int AdminDuration { get; set; }

        [Required]
        public Guid StudyRoomSessionId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var list = new[] { StudentPay, SpitballPay };

            foreach (var @decimal in list)
            {
                if (@decimal == 0)
                {
                    continue;
                }

                if (@decimal < 5 || @decimal > 50000)
                {
                    yield return new ValidationResult(
                            "Invalid Price");
                }
            }
        }
    }


}
