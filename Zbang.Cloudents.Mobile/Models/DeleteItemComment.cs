using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class DeleteItemComment
    {
        //[Required]
        //public string ItemUid { get; set; }

        [Required]
        public long CommentId { get; set; }

        
    }
}