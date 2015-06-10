﻿using System.ComponentModel.DataAnnotations;
using System.Text;
using Zbang.Cloudents.Mobile.Models.Account.Resources;
using Zbang.Cloudents.Mobile.Models.Resources;
using Zbang.Cloudents.SiteExtension;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.Mobile.Models.Account
{
    public class Register
    {
        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "FieldRequired")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotValid")]
        [RegularExpression(Validation.EmailRegexWithTrailingEndingSpaces, ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "EmailNotCorrect")]
        [Display(ResourceType = typeof(RegisterResources), Name = "EmailAddress")]
        public string NewEmail { get; set; }


        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "PwdRequired")]
        [ValidatePasswordLength(ErrorMessageResourceName = "MustBeAtLeast", ErrorMessageResourceType = typeof(ValidatePasswordResources))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(RegisterResources), Name = "Password")]
        public string Password { get; set; }




        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "GenderRequired")]
        public bool? IsMale { get; set; }

       
        public long UniversityId { get; set; }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("FirstName " + FirstName);
            sb.AppendLine("LastName " + LastName);
            sb.AppendLine("NewEmail " + NewEmail);
            sb.AppendLine("IsMale " + IsMale);
            
            return sb.ToString();
        }
    }
}