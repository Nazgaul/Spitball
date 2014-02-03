using System.ComponentModel.DataAnnotations;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Zbox.WebApi.Models
{
    public class AddFileToBox
    {
        [Required]
        [StringLength(Domain.Item.NameLength)]
        [RegularExpression(Validation.WindowFileRegex)]
        public string FileName { get; set; }
        [Required]
        public string BlobName { get; set; }

        public override string ToString()
        {
            return string.Format("  FileName {0} BlobName {1}",
                   FileName, BlobName);
        }
    }
}