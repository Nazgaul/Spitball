using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Quiz Id : " + QuizId);
            sb.AppendLine("Box Id : " + BoxId);
            sb.AppendLine("Box Name: " + BoxName);
            sb.AppendLine("Quiz name: " + QuizName);
            sb.AppendLine("Univerisity name  " + UniversityName);
            return sb.ToString();
        }
    }
}