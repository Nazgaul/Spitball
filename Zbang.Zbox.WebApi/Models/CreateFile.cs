
using System.ComponentModel.DataAnnotations;


namespace Zbang.Zbox.WebApi.Models
{
    public class CreateFile
    {
        [Required]
        public string BlobName { get; set; }

        public override string ToString()
        {
            return string.Format("  BlobName {0}",
                    BlobName);
        }
    }
}