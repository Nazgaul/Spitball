using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    [Bind(Exclude = "Code")]
    [Serializable]
    public class ChangeMail
    {
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "EmailEmpty")]
        //[RegularExpression(Validation.EmailRegex, ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "EmailNotCorrect")]
        [EmailVerify(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "EmailNotCorrect")]
        public string Email { get; set; }

        public int? Code { get; set; }

        public DateTime TimeOfExpire { get; set; }

        public override string ToString()
        {
            return $"email: {Email} code: {Code}";
        }
    }
}