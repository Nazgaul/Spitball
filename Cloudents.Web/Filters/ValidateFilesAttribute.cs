using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Filters
{
    public class ValidateFilesAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IEnumerable<string> t)
            {
                if (t.Count() < 4)
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult(GetErrorMessage());
        }

        private string GetErrorMessage()
        {
            return "Maximum 4 files";
        }

    }
}
