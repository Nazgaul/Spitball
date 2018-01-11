using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class AddCommentRequest
    {
        public string Content { get; set; }

        [Required]
        public bool Anonymous { get; set; }

        public IEnumerable<long> FilesIds { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}