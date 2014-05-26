using System.ComponentModel.DataAnnotations;

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