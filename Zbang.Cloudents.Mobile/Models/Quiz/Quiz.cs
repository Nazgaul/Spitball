using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class Quiz
    {
        [Required]
        public long BoxId { get; set; }

        [AllowHtml]
        public string Name { get; set; }
    }
}