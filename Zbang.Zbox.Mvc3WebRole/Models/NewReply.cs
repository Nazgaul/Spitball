using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zbang.Zbox.Mvc3WebRole.Models
{
    public class NewReply
    {
        [Required]
        public long CommentToReplyId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [AllowHtml]
        public string CommentText { get; set; }
        
        [Required]
        public string BoxUId { get; set; }
    }
}