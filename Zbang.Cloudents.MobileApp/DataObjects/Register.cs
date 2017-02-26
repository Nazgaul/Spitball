
using System.ComponentModel.DataAnnotations;
using System.Text;
using Zbang.Cloudents.MobileApp.Filters;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class Register
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailVerify]
        public string NewEmail { get; set; }


        [Required]
        public string Password { get; set; }

        [Required]
        public string Culture { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("FirstName " + FirstName);
            sb.AppendLine("LastName " + LastName);
            sb.AppendLine("NewEmail " + NewEmail);
            return sb.ToString();
        }
    }
}