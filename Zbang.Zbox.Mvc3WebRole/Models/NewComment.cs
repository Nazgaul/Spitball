using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zbang.Zbox.Mvc3WebRole.Models
{
    public class NewComment
    {
       
        [Required]        
        [DataType(DataType.Text)]  
        [AllowHtml]
        public string CommentText { get; set; }

        [Required]
        public string BoxUid { get; set; }

       
        public string ItemUid { get; set; }


        public override string ToString()
        {
            return string.Format("CommentText {0} BoxUid {1} ItemUid {2}", CommentText, BoxUid, ItemUid);
        }
    }
}