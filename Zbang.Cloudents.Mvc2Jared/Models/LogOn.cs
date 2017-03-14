using System.ComponentModel.DataAnnotations;
namespace Zbang.Cloudents.Mvc2Jared.Models
{
    public class LogOn
    {
        public LogOn()
        {
            RememberMe = true;
        }
        //Error message = null , bug 5416 http://stackoverflow.com/questions/12474876/either-errormessagestring-or-errormessageresourcename-must-be-set-but-not-both
        [EmailAddress(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "NotValidEmail", ErrorMessage = null)]
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