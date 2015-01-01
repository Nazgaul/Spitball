using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mobile.Models.Resources;
using Zbang.Zbox.Domain;

namespace Zbang.Cloudents.Mobile.Models
{
    public class CreateBox
    {
        
        [Required(ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(CreateBoxResources), Name = "BoxName")]
        [StringLength(Box.NameLength, ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "BoxNameUpTo")]
        //[RegularExpression(Validation.WindowFileRegex, ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "BoxNameInvalidChar")]
        //[Remote("BoxNameDuplicate", "Boxes", ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "BoxNameExists", HttpMethod = "POST")]
        public string BoxName { get; set; }

        //[StringLength(Box.DescriptionLength)]
        //[Display(ResourceType = typeof(CreateBoxResources), Name = "BoxDescription")]


       // public BoxPrivacySettings privacySettings { get; set; }
        //public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("BoxName {0} ", BoxName);
        }
    }
}