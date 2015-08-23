using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class SaveUserAnswersRequests
    {
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public long QuizId { get; set; }
        public IEnumerable<UserAnswer> Answers { get; set; }

        [Required]
        public long BoxId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("start time: " + StartTime);
            sb.AppendLine("end time: " + EndTime);
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