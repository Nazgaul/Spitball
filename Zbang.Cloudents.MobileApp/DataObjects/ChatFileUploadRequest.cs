using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class ChatFileUploadRequest
    {
        [Required]
        public string BlobName { get; set; }
        [Required]
        public IList<long> Users { get; set; }

    }
}