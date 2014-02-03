using System.ComponentModel.DataAnnotations;

using Zbang.Zbox.Domain.Common;

namespace Zbang.Zbox.WebApi.Models
{
    public class ChangeFileName
    {
        [Required]
        [StringLength(Domain.Item.NameLength)]
        [RegularExpression(Validation.WindowFileRegex)]
        public string NewFileName { get; set; }

        public override string ToString()
        {
            return string.Format("  NewFileName {0}",
                    NewFileName);

        }
    }
}