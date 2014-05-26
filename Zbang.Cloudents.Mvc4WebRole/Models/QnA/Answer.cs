using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models.QnA
{
    public class Answer
    {
        public Answer()
        {
            Files = new long[0];
        }
        [Required]
        [AllowHtml]
        public string Content { get; set; }
        [Required]
        public long BoxUid { get; set; }

        [Required]
        public Guid QuestionId { get; set; }


        public long[] Files { get; set; }

    }
}