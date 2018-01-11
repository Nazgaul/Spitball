using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Mobile.Models
{
    public class ChatFileUploadRequest
    {
        [Required]
        public string BlobName { get; set; }
        [Required]
        public IList<long> Users { get; set; }
    }
}