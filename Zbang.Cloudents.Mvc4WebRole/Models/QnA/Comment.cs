using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models.QnA
{
    public class Comment
    {
        public string Content { get; set; }
        [Range(1,long.MaxValue)]
        public long BoxId { get; set; }

        public long[] Files { get; set; }

        public bool Anonymously { get; set; }
    }
}