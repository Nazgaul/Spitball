﻿using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mobile.Models.Account.Resources;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.Mobile.Models.Account
{
    public class LogOn
    {
        public LogOn()
        {
            RememberMe = true;
        }

        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(LogOnResources), Name = "EmailAddress")]
        [Required(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "EmailRequired")]
        [RegularExpression(Validation.EmailRegexWithTrailingEndingSpaces, ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "NotValidEmail")] 
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "PwdAtLeast6Chars")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(LogOnResources), Name = "Password")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(LogOnResources), Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}