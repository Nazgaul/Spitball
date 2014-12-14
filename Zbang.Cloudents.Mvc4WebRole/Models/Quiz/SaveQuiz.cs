using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Quiz
{
    public class SaveQuiz
    {
        public long QuizId { get; set; }
       

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Quiz Id : " + QuizId);
            return sb.ToString();
        }
    }
}