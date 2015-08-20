
using System.ComponentModel.DataAnnotations;
using System.Text;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class Register
    {
        [Required]//(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        public string FirstName { get; set; }

        [Required]//(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        public string LastName { get; set; }

        [Required]//(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotValid")]
        [RegularExpression(Validation.EmailRegexWithTrailingEndingSpaces)]//, ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotCorrect")]
        public string NewEmail { get; set; }


        [Required]//(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "PwdRequired")]
        //[ValidatePasswordLength(ErrorMessageResourceName = "MustBeAtLeast", ErrorMessageResourceType = typeof(ValidatePasswordResources))]
        public string Password { get; set; }

        [Required]
        public string Culture { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("FirstName " + FirstName);
            sb.AppendLine("LastName " + LastName);
            sb.AppendLine("NewEmail " + NewEmail);
            return sb.ToString();
        }
    }
}