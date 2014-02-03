using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models.QnA
{
    public class Question
    {
        public Question()
        {
            Files = new long[0];
        }
        [Required]
        [AllowHtml]
        public string Content { get; set; }
        [Required]
        public long BoxUid { get; set; }

        public long[] Files { get; set; }
    }
}