using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class FileUploadRequest
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public string BlobName { get; set; }
        [Required]
        public string BoxId { get; set; }
        public bool Question { get; set; }
    }
}