using System.Collections.Generic;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;

namespace Cloudents.Core.DTOs
{
    public class EmailDto
    {
        public EmailDto()
        {
            Blocks = new List<EmailBlockDto>();
        }

        public string ToEmailAddress { get; set; }

        public Language Language { get; set; }
        public bool SocialShare { get; set; }
        public string Subject { get; set; }

        public IList<EmailBlockDto> Blocks { get; set; }

        public long UserId { get; set; }
    }

    public class DocumentPurchaseEmailDto : EmailDto
    {
        public string CourseName { get; set; }
        public string DocumentName { get; set; }
        public decimal Tokens { get; set; }
    }

    public class AnswerAcceptedEmailDto : EmailDto
    {
        private string _questionText;
        private string _answerText;

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

        public decimal Tokens { get; set; }
    }

    public class EmailBlockDto 
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Body { get; set; }
        public string Cta { get; set; }
        public string Url { get; set; }
        public string MinorTitle{ get; set; }
    }
}