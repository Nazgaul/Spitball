using System.ComponentModel.DataAnnotations;
using System.Text;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class Register
    {
        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotValid")]
        [ValidateEmail(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "NotValidEmail")]
        public string NewEmail { get; set; }


        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "PwdRequired")]
        [ValidatePasswordLength(ErrorMessageResourceName = "MustBeAtLeast", ErrorMessageResourceType = typeof(ValidatePasswordResources))]
        [DataType(DataType.Password)]
        //[Display(ResourceType = typeof(RegisterResources), Name = "Password")]
        public string Password { get; set; }

        public long? UniversityId { get; set; }

        public string ReturnUrl { get; set; }

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