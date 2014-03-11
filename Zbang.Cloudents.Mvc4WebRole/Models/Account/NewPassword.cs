using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class NewPassword
    {
        [Required(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "PwdAtLeast6Chars")]
        [ValidatePasswordLength(ErrorMessageResourceName = "PwdAtLeast6Chars", ErrorMessageResourceType = typeof(LogOnResources))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(LogOnResources), Name = "Password")]
        public string Password { get; set; }

        [Required]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "ConfirmEmailCompare")]
        [Display(ResourceType = typeof(RegisterResources), Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}