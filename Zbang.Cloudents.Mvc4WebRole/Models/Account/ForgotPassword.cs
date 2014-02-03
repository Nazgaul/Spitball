using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class ForgotPassword
    {
        [Required(ErrorMessageResourceType = typeof(ForgotPasswordResources), ErrorMessageResourceName = "EmailNotMatch")]
        [Display(ResourceType = typeof(ForgotPasswordResources), Name = "YourEmail")]
        [RegularExpression(Validation.EmailRegexWithTrailingEndingSpaces, ErrorMessageResourceType = typeof(ForgotPasswordResources), ErrorMessageResourceName = "EmailNotValid")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}