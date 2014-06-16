using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class SaveUserAnswers
    {
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public long QuizId { get; set; }
        public IEnumerable<UserAnswer> Answers { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("start time:{0} end time: {1} quizId {2} answers {3}", StartTime, EndTime, QuizId, String.Join("\n",Answers));

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
            var sb = new StringBuilder();
            sb.AppendFormat("QuestionId: {0} AnswerId: {1}", QuestionId, AnswerId);
            return sb.ToString();
        }
    }
}