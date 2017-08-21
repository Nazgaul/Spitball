using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models.QnA
{
    public class Reply
    {
        //[AllowHtml]
        public string Content { get; set; }
        [Range(1, long.MaxValue)]
        public long BoxId { get; set; }

        //TODO: Guid can be empty
        [Required]
        public Guid CommentId { get; set; }

        public long[] Files { get; set; }

        public override string ToString()
        {
            return string.Format("Content: {0} BoxUid {1} QuestionId {2}", Content, BoxId, CommentId);
        }
    }
}