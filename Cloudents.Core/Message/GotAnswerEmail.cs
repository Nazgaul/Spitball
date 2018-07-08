using System;
using Cloudents.Core.Extension;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class GotAnswerEmail : BaseEmail
    {
        public GotAnswerEmail(string questionText, string to, string answerText, string link) : base(to, "Answer", "You got an answer to question you asked")
        {
            QuestionText = questionText.Truncate(40, true);
            AnswerText = answerText.Truncate(40, true);
            Link = link;
        }

        public string QuestionText { get; private set; }

        public string AnswerText { get; private set; }

        public string Link { get; private set; }

        public override string ToString()
        {
            return $"You got an answer to question: {QuestionText}";
        }
    }
}