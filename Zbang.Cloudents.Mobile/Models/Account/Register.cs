using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mobile.Models.Account.Resources;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings;
using Zbang.Cloudents.Mobile.Models.Resources;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class Register
    {
        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotValid")]
        [RegularExpression(Validation.EmailRegexWithTrailingEndingSpaces, ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotCorrect")]
        [Display(ResourceType = typeof(RegisterResources), Name = "EmailAddress")]
        public string NewEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(RegisterResources), Name = "ConfirmEmail")]
        //the new version doesn't get the resource
        [System.Web.Mvc.Compare("NewEmail", ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "ConfirmEmailCompare")]
        public string ConfirmEmail { get; set; }


        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "PwdRequired")]
        [ValidatePasswordLength(ErrorMessageResourceName = "MustBeAtLeast", ErrorMessageResourceType = typeof(ValidatePasswordResources))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(RegisterResources), Name = "Password")]
        public string Password { get; set; }




        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "GenderRequired")]
        public bool? IsMale { get; set; }

        [Required]
        [Display(Name="מאשר קבלת תוכן שיווקי מהתאחדות הסטודנטים הארצית")]
        public bool MarketEmail { get; set; }

        public UserLanguage Language { get; set; }

        public long? UniversityId { get; set; }

        public string ReturnUrl { get; set; }

        public long? BoxId { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("FirstName " + FirstName);
            sb.AppendLine("LastName " + LastName);
            sb.AppendLine("NewEmail " + NewEmail);
            sb.AppendLine("IsMale " + IsMale);
            sb.AppendLine("MarketEmail " + MarketEmail);
            sb.AppendLine("Language " + Language.Language);
            return sb.ToString();
        }
    }
}