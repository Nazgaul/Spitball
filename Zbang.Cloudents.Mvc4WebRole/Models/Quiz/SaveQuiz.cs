using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class SaveQuiz
    {
        public long QuizId { get; set; }
        public long BoxId { get; set; }
        [Required]
        public string BoxName { get; set; }
        [Required]
        public string QuizName { get; set; }
        [Required]
        public string UniversityName { get; set; }
    }
}