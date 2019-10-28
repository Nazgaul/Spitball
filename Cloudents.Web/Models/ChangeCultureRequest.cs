using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Web.Resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Models
{
    public class ChangeCultureRequest : IValidatableObject
    {
        [Required]
        public RequestCulture Culture { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Language.SystemSupportLanguage().Any(a=>
                {
                    CultureInfo b = a;
                    return Culture.Culture.Equals(b);
                })) 
            {
                var stringLocalizer = validationContext.GetService(typeof(IStringLocalizer<DataAnnotationSharedResource>)) as IStringLocalizer<DataAnnotationSharedResource>;
                var errorMessage = "Invalid Culture";
                if (stringLocalizer != null)
                {
                    errorMessage = stringLocalizer["InvalidCulture"];
                }

                yield return new ValidationResult(
                    errorMessage,
                    new[] { nameof(Culture) });
            }
        }
    }
}
