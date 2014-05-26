using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Tabs
{
    public class CreateBoxItemTab
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(ResourceType = typeof(CreateBoxResources), Name = "TabName")]
        public string Name { get; set; }

        [Required]
        public long BoxId { get; set; }

        public override string ToString()
        {
            return string.Format("CreateBoxTab Name: {0}", Name);
        }
    }
}