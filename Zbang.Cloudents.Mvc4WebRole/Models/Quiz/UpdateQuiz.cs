using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class UpdateQuiz
    {
        [Required]
        public long Id { get; set; }       
        public string Name { get; set; }
    }
}