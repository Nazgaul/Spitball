
using System.ComponentModel.DataAnnotations;


namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class FileUploadRequest
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public string BlobName { get; set; }
        [Required]
        public string BoxId { get; set; }
        [Required]
        public string Size { get; set; }
    }
}