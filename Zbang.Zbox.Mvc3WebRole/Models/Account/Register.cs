using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Mvc3WebRole.Models.Account.Resources;
using Zbang.Zbox.Mvc3WebRole.Models.Resources;

namespace Zbang.Zbox.Mvc3WebRole.Models
{
    public class Register
    {
        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(RegisterResources), Name = "UserName")]
        [RegularExpression(@"^[^@]*$", ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "NameCannotContain")]
        public string NewUserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotValid")]
        [RegularExpression(Validation.EmailRegex, ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotCorrect")]
        [Display(ResourceType = typeof(RegisterResources), Name = "EmailAddress")]
        public string NewEmail { get; set; }


        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "PwdRequired")]
        [ValidatePasswordLengthAttribute(ErrorMessageResourceName = "MustBeAtLeast", ErrorMessageResourceType = typeof(ValidatePasswordResources))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(RegisterResources), Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FirstConfirm")]
        [Display(ResourceType = typeof(RegisterResources), Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "ConfirmPasswordComapre")]
        public string ConfirmPassword { get; set; }

        [Display(ResourceType = typeof(RegisterResources), Name = "University")]
        public string University { get; set; }
        //[Required(ErrorMessageResourceName = "AgreeToTerm", ErrorMessageResourceType = typeof(RegisterResources))]
        //[RegularExpression("true|True", ErrorMessageResourceName = "AgreeToTerm", ErrorMessageResourceType = typeof(RegisterResources))]
        //public bool AgreeToTerm { get; set; }


        //[ValidatePasswordLengthAttribute]
        //[DataType(DataType.Password)]
        //[Display(ResourceType = typeof(RegisterResources), Name = "Password")]
        //public string NewPasswordText { get; set; }


    }
}