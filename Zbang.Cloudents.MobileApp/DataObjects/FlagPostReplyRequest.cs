using System;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class FlagPostReplyRequest
    {
        [Required]
        public Guid PostId { get; set; }
    }
}