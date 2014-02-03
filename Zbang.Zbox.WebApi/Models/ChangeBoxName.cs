
using System.ComponentModel.DataAnnotations;


namespace Zbang.Zbox.WebApi.Models
{
    public class ChangeBoxName
    {
        [Required]
        [StringLength(Zbang.Zbox.Domain.Box.NameLength)]
       // [RegularExpression(Validation.WindowFileRegex)]
        public string NewBoxName { get; set; }

        public override string ToString()
        {
            return string.Format("  newBoxName {0}",
                    NewBoxName);

        }
    }
}