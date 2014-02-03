using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class CreateBoxTab
    {
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("CreateBoxTab Name: {0}", Name);
        }
    }
}