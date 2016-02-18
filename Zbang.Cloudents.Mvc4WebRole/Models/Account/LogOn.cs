using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class LogOn
    {
        public LogOn()
        {
            RememberMe = true;
        }

        [EmailAddress(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "NotValidEmail")]
        [Required(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "EmailRequired")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "PwdAtLeast6Chars")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Display(ResourceType = typeof(LogOnResources), Name = "RememberMe")]
        public bool RememberMe { get; set; }

        public override string ToString()
        {
            return "Email: " + Email + " Password: " + Password;
        }
    }
}