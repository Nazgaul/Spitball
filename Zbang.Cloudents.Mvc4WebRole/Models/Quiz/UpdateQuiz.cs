using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class UpdateQuiz
    {
        [Required]
        public long Id { get; set; }  
        [AllowHtml]
        public string Name { get; set; }
    }
}