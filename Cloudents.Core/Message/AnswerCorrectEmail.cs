using System;
using System.Collections.Generic;
using System.Globalization;
using Cloudents.Core.Extension;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class AnswerCorrectEmail : BaseEmail
    {
        public AnswerCorrectEmail(string to, 
            string questionText, string answerText,
            string link, decimal tokens, CultureInfo info) 
            : base(to,  "Congratulations, your answer has been accepted",
                info)
        {
            QuestionText = questionText.Truncate(40, true);
            AnswerText = answerText.Truncate(40, true);
            Link = link;
            Tokens = tokens.ToString("#.##");
        }

        public string QuestionText { get; private set; }

        public string AnswerText { get; private set; }

        public string Link { get; private set; }

        public string Tokens { get; private set; }

        public override string ToString()
        {
            return "Congratz one of your answers is correct";
        }

        public override string Campaign => "AnswerCorrect";
        protected override IDictionary<CultureInfo, string> Templates => new Dictionary<CultureInfo, string>()
        {
            { Language.Hebrew.Culture,"1b20fe5d-6e32-441f-9870-f309112aca33"},
            {Language.English.Culture ,"ae7e5e65-7224-4e82-a830-e83d22c211d4" }
        };
    }
}