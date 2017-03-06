using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class SaveUserAnswersRequests
    {
        [Required]
        public long NumberOfSeconds { get; set; }
        [Required]
        public long QuizId { get; set; }
        public IEnumerable<UserAnswer> Answers { get; set; }

        [Required]
        public long BoxId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("quizId: " + QuizId);
            if (Answers != null) sb.AppendLine("answers: " + string.Join("\n", Answers));

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
            return $"QuestionId: {QuestionId} AnswerId: {AnswerId}";
        }
    }
}