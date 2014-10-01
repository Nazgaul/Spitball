using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models.QnA
{
    public class Question
    {
        [AllowHtml]
        public string Content { get; set; }
        [Required]
        public long BoxId { get; set; }

        public long[] Files { get; set; }
    }
}