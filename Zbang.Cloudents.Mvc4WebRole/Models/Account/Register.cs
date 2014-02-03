using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class Register : IModelBinder
    {

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(RegisterResources), Name = "Name")]
        [RegularExpression(@"^[^@]*$", ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "NameCannotContain")]
        public string NewUserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotValid")]
        [RegularExpression(Validation.EmailRegexWithTrailingEndingSpaces, ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotCorrect")]
        [Remote("CheckEmail","Account",HttpMethod="Post",ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotValid")]
        [Display(ResourceType = typeof(RegisterResources), Name = "EmailAddress")]
        public string NewEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(RegisterResources), Name = "ConfirmEmail")]
        //the new version doesnt get the resouce
        [System.Web.Mvc.Compare("NewEmail", ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "ConfirmEmailCompare")]
        public string ConfirmEmail { get; set; }


        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "PwdRequired")]
        [ValidatePasswordLength(ErrorMessageResourceName = "MustBeAtLeast", ErrorMessageResourceType = typeof(ValidatePasswordResources))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(RegisterResources), Name = "Password")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FirstConfirm")]
        //[Display(ResourceType = typeof(RegisterResources), Name = "ConfirmPassword")]
        //[Compare("Password", ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "ConfirmPasswordComapre")]
        //public string ConfirmPassword { get; set; }


        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            
            var x = BindModel(controllerContext, bindingContext);
            
            return x;
        }
    }
}