using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class Question
    {
        [Required]
        public long QuizId { get; set; }
        [Required]
        public string Text { get; set; }
    }
}