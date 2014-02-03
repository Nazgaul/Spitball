using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class DeleteAnnotation
    {
        //[Required]
        //public string ItemUid { get; set; }

        [Required]
        public long CommentId { get; set; }

        
    }
}