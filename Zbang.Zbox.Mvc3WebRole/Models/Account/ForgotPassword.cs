using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Mvc3WebRole.Models.Account.Resources;

namespace Zbang.Zbox.Mvc3WebRole.Models.Account
{
    public class ForgotPassword
    {
        [Required(ErrorMessageResourceType = typeof(ForgotPasswordResources), ErrorMessageResourceName = "EmailNotMatch")]
        [Display(ResourceType = typeof(ForgotPasswordResources), Name = "YourEmail")]
        [RegularExpression(Validation.EmailRegex, ErrorMessageResourceType = typeof(ForgotPasswordResources), ErrorMessageResourceName = "EmailNotValid")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}