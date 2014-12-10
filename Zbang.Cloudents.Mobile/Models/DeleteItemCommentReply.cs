using System.ComponentModel.DataAnnotations;


namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class DeleteItemCommentReply
    {
        [Required]
        public long ReplyId { get; set; }
    }
}