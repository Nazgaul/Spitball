using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class Profile 
    {
        //[Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "UsernameEmpty")]
        //[DataType(DataType.Text)]
        //[Display(ResourceType = typeof(AccountSettingsResources), Name = "Name")]
        //[RegularExpression(@"^[^@]*$", ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "NameCannotContain")]
        //public string Name { get; set; }

        //public string Language { get; set; }

        //readonly fields
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "School")]
        public string University { get; set; }
        public string UniversityImage { get; set; }

        //TODO add validation to Image url
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Photo")]
        public string Image { get; set; }

        //TODO add validation to Image url
        public string LargeImage { get; set; }



        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "UsernameEmpty")]
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "LastNameRequired")]
        public string LastName { get; set; }

        //public long? UniversityCode { get; set; }


        //public System.Collections.Generic.IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (!Zbang.Zbox.Infrastructure.Culture.Languages.CheckIfLanguageIsSupported(Language))
        //    {
        //        yield return new ValidationResult(AccountSettingsResources.LanguageNotSupported, new[] { "Language" });
        //    }
        //}
    }
}