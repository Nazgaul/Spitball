﻿using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class PhoneNumberRequest
    {
        [Required]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$")]
        public string Number { get; set; }

        public override string ToString()
        {
            return $"{nameof(Number)}: {Number}";
        }
    }

    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
