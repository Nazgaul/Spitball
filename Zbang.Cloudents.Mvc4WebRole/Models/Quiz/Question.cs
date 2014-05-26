using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class Question
    {
        [Required]
        public long QuizId { get; set; }
        public string Text { get; set; }
    }
}