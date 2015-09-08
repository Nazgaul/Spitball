using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class DiscussionRequest
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public Guid QuestionId { get; set; }
    }
}