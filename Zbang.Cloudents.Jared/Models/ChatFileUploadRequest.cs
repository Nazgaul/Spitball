using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class ChatFileUploadRequest
    {
        [Required]
        public string BlobName { get; set; }
        [Required]
        public IList<long> Users { get; set; }
    }
}