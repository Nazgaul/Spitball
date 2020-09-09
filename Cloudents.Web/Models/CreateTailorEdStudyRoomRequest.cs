using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Web.Resources;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Models
{
    public class CreateTailorEdStudyRoomRequest : IValidatableObject
    {
        [Range(1,50)]
        public int AmountOfUsers { get; set; }

        public string Culture { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var culture = new CultureInfo(Culture);
            if (!Language.SystemSupportLanguage().Any(a =>
            {
                
                CultureInfo b = a;
                
                return culture.Equals(b);
            }))
            {
                var errorMessage = "Invalid Culture";
                yield return new ValidationResult(
                    errorMessage,
                    new[] { nameof(Culture) });
            }
        }
    }

    public class EnterTailorEdRoomRequest
    {
        [Required] public string Code { get; set; }
    }

}