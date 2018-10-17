using Cloudents.Core.Enum;

namespace Cloudents.Core.Storage.Dto
{
    /// <summary>
    /// New message of to pass to azure function to invoke new create question message.
    /// NOTE: you cannot pass blob because it will not work on azure function.
    /// </summary>
    public class NewQuestionMessage
    {
        public NewQuestionMessage(int subjectId, string text, decimal price,long userId)
        {
            SubjectId = (QuestionSubject)subjectId;
            Text = text;
            Price = price;
            UserId = userId;
        }

        public NewQuestionMessage(QuestionSubject subjectId, string text, decimal price, long userId)
        {
            SubjectId = subjectId;
            Text = text;
            Price = price;
            UserId = userId;
        }

        public NewQuestionMessage()
        {
            
        }

        public QuestionSubject SubjectId { get; set; }
        public string Text { get; set; }

        public decimal Price { get; set; }

        public long UserId { get; set; }
    }
}