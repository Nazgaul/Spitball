using System.ComponentModel.DataAnnotations;

namespace Cloudents.Mobile.Models
{
    public class DropboxUploadRequest
    {
        [Required]
        public long BoxId { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Question { get; set; }
    }
}