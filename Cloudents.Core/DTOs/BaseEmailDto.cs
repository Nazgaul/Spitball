using System;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using System.Globalization;

namespace Cloudents.Core.DTOs
{
    public class EmailDto
    {
        [EntityBind(nameof(User.Email))]
        public string ToEmailAddress { get; set; }
        [EntityBind(nameof(User.Language))]
        public string Language { get; set; }


        [EntityBind(nameof(User.Id))]
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

    public class UpdateEmailDto
    {
        //public long Id { get; set; }
        public string UserName { get; set; }

        public string ToEmailAddress { get; set; }
        public CultureInfo Language { get; set; }

        public long UserId { get; set; }

        public DateTime Since { get; set; }
        // public int NumUpdates { get; set; }
        // public int XQuestions { get; set; }
        // public int XNewItems { get; set; }
        // public IEnumerable<DocumentEmailDto> Documents { get; set; }
        // public IEnumerable<QuestionEmailDto> Questions { get; set; }
        // public string To { get; set; }
    }

    public class QuestionEmailDto
    {
        public long UserId { get; set; }
        public long QuestionId { get; set; }
        public string UserImage { get; set; }
        public string UserName { get; set; }
        public string QuestionText { get; set; }
    }
    public class DocumentEmailDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
    }
}