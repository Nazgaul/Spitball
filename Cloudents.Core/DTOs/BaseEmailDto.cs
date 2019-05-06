using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;

namespace Cloudents.Core.DTOs
{
    public class EmailDto
    {
        [EntityBind(nameof(RegularUser.Email))]
        public string ToEmailAddress { get; set; }
        [EntityBind(nameof(RegularUser.Language))]
        public string Language { get; set; }

        
        [EntityBind(nameof(RegularUser.Id))]
        public long UserId { get; set; }
    }

    public class DocumentPurchaseEmailDto : EmailDto
    {
        [EntityBind(nameof(Document.Course.Id))]
        public string CourseName { get; set; }
        [EntityBind(nameof(Document.Name))]
        public string DocumentName { get; set; }
        [EntityBind(nameof(Transaction.Price))]
        public decimal Tokens { get; set; }
    }

    public class AnswerAcceptedEmailDto : EmailDto
    {
        [EntityBind(nameof(Question.Text))]
        private string _questionText;
        [EntityBind(nameof(Answer.Text))]
        private string _answerText;

        [EntityBind(nameof(Question.Id))]
        public long QuestionId { get; set; }

        public string QuestionText
        {
            get => _questionText.Replace("\n", "<br>").Truncate(40, true);
            set => _questionText = value;
        }

        public string AnswerText
        {
            get => _answerText.Replace("\n", "<br>").Truncate(40, true);
            set => _answerText = value;
        }

        //public decimal Tokens { get; set; }
    }


}