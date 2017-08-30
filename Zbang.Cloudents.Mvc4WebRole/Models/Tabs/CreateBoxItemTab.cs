using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Tabs
{
    public class CreateBoxItemTab
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public long BoxId { get; set; }

        public override string ToString()
        {
            return string.Format("CreateBoxTab Name: {0}", Name);
        }
    }
}