using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models.QnA
{
    public class Answer
    {
        [Required]
        [AllowHtml]
        public string Content { get; set; }
        [Required]
        public long BoxId { get; set; }

        [Required]
        public Guid QuestionId { get; set; }


        public long[] Files { get; set; }

        public override string ToString()
        {
            return string.Format("Content: {0} BoxUid {1} QuestionId {2}", Content, BoxId, QuestionId);
        }

    }
}