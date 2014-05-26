﻿using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class ReplyAnnotation
    {
        [Required(AllowEmptyStrings = false)]
        public string Comment { get; set; }

        [Required]
        public long ItemId { get; set; }
        [Required]
        public int ImageId { get; set; }

        [Required]
        public long CommentId { get; set; }
    }
}