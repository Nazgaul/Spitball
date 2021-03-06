﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class PaymentRequest : IValidatableObject
    {
        public double StudentPay { get; set; }
        //[Range(5, 50000)]
        public double SpitballPay { get; set; }

        public long UserId { get; set; }
        public long TutorId { get; set; }

        public int AdminDuration { get; set; }

        [Required]
        public Guid StudyRoomSessionId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var list = new[] { StudentPay, SpitballPay };
            if (StudentPay < 5 && SpitballPay < 5)
            {
                yield return new ValidationResult(
                    "Invalid Price");
            }
            foreach (var price in list)
            {
                if (price.CompareTo(0) == 0)
                {
                    continue;
                }

                if (price < 5 || price > 50000)
                {
                    yield return new ValidationResult(
                            "Invalid Price");
                }
            }
        }
    }


}
