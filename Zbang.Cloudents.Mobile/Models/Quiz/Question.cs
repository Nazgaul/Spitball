using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class Question
    {
        [Required]
        public long QuizId { get; set; }
        [AllowHtml]
        public string Text { get; set; }
    }
}