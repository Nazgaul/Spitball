using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class ForgotPassword
    {
        [Required(ErrorMessageResourceType = typeof(ForgotPasswordResources), ErrorMessageResourceName = "EmailNotMatch")]
        [EmailAddress(ErrorMessageResourceType = typeof(ForgotPasswordResources), ErrorMessageResourceName = "EmailNotValid")]
        public string Email { get; set; }


        public override string ToString()
        {
            return "Email: " + Email;
        }
    }
}