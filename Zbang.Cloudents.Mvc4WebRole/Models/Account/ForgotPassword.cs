using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class ForgotPassword
    {
        [Required(ErrorMessageResourceType = typeof(ForgotPasswordResources), ErrorMessageResourceName = "EmailNotMatch")]
        //Error message = null , bug 5416 http://stackoverflow.com/questions/12474876/either-errormessagestring-or-errormessageresourcename-must-be-set-but-not-both
        [EmailAddress(ErrorMessageResourceType = typeof(ForgotPasswordResources), ErrorMessageResourceName = "EmailNotValid", ErrorMessage = null)]
        public string Email { get; set; }

        public override string ToString()
        {
            return "Email: " + Email;
        }
    }
}