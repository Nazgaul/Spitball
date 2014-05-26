using System;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class Discussion
    {
        [Required]
        public Guid QuestionId { get; set; }
        [Required]
        public string Text { get; set; }
    }
}