using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    [Bind(Exclude = "Code")]
    [Serializable]
    public class ChangeMail
    {
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "EmailEmpty")]
        [RegularExpression(Validation.EmailRegex, ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "EmailNotCorrect")]
        public string Email { get; set; }

        public int? Code { get; set; }

        public DateTime TimeOfExpire { get; set; }

    }
}