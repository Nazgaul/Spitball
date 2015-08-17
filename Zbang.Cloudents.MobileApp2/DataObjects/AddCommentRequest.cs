using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class AddCommentRequest
    {
        [Required]
        public string Content { get; set; }

        public IEnumerable<long> FileIds { get; set; }

        public bool Anonymously { get; set; }
    }
}