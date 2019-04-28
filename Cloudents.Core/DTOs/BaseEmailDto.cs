using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;

namespace Cloudents.Core.DTOs
{
    public class EmailDto
    {
        [DtoToEntityConnection(nameof(RegularUser.Email))]
        public string ToEmailAddress { get; set; }
        [DtoToEntityConnection(nameof(RegularUser.Language))]
        public string Language { get; set; }

        
        [DtoToEntityConnection(nameof(RegularUser.Id))]
        public long UserId { get; set; }
    }

    public class DocumentPurchaseEmailDto : EmailDto
    {
        [DtoToEntityConnection(nameof(Document.Course.Id))]
        public string CourseName { get; set; }
        [DtoToEntityConnection(nameof(Document.Name))]
        public string DocumentName { get; set; }
        [DtoToEntityConnection(nameof(Transaction.Price))]
        public decimal Tokens { get; set; }
    }

    public class AnswerAcceptedEmailDto : EmailDto
    {
        [DtoToEntityConnection(nameof(Question.Text))]
        private string _questionText;
        [DtoToEntityConnection(nameof(Answer.Text))]
        private string _answerText;

        [DtoToEntityConnection(nameof(Question.Id))]
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