using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;
using Zbang.Zbox.Domain;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class CreateBox
    {
        [Required(ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(CreateBoxResources), Name = "BoxName")]
        [StringLength(Box.NameLength, ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "BoxNameUpTo")]
        public string BoxName { get; set; }
        public override string ToString()
        {
            return string.Format("BoxName {0} ", BoxName);
        }
    }
}