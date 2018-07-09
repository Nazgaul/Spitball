using System;
using Cloudents.Core.Extension;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class AnswerCorrectEmail : BaseEmail
    {
        public AnswerCorrectEmail(string to, string questionText, string answerText, string link, decimal tokens) 
            : base(to, "accepted-answer", "Congratulations, your answer has been accepted")
        {
            QuestionText = questionText.Truncate(40, true);
            AnswerText = answerText.Truncate(40, true);
            Link = link;
            Tokens = tokens;
        }

        public string QuestionText { get; private set; }

        public string AnswerText { get; private set; }

        public string Link { get; private set; }

        public decimal Tokens { get; private set; }

        public override string ToString()
        {
            return $"Congratz one of your answers is correct";
        }
    }
}