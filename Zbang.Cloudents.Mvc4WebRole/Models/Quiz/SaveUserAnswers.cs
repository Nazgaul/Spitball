using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class SaveUserAnswers
    {
        [Required]
        public long NumberOfMilliseconds { get; set; }
        [Required]
        public long QuizId { get; set; }
        public IEnumerable<UserAnswer> Answers { get; set; }

        [Required]
        public long BoxId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("quizId: " + QuizId);
            if (Answers != null) sb.AppendLine("answers: " + String.Join("\n", Answers));

            return sb.ToString();
        }
    }

    public class UserAnswer
    {
        [Required]
        public Guid QuestionId { get; set; }
        [Required]
        public Guid AnswerId { get; set; }

        public override string ToString()
        {
            return string.Format("QuestionId: {0} AnswerId: {1}", QuestionId, AnswerId);
        }
    }
}