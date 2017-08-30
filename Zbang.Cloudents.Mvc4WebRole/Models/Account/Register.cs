using System.ComponentModel.DataAnnotations;
using System.Text;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class Register
    {
        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotValid")]
        //Error message = null , bug 5416 http://stackoverflow.com/questions/12474876/either-errormessagestring-or-errormessageresourcename-must-be-set-but-not-both
        //[EmailAddress(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "NotValidEmail", ErrorMessage = null)]
        [EmailVerify(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "NotValidEmail", ErrorMessage = null)]
        public string NewEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "PwdRequired")]
        [ValidatePasswordLength(ErrorMessageResourceName = "MustBeAtLeast", ErrorMessageResourceType = typeof(ValidatePasswordResources))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        public Sex Sex { get; set; }

        //public long? UniversityId { get; set; }

        //public string ReturnUrl { get; set; }

        public long? BoxId { get; set; }

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