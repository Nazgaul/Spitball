﻿using Cloudents.Core.Extension;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class GotAnswerEmail : BaseEmail
    {
        public GotAnswerEmail(string questionText, string to, string answerText, string link, CultureInfo info)
            : base(to, "Someone has answered your question", info)
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

        public override string Campaign => "You got an answer";
        protected override IDictionary<CultureInfo, string> Templates => new Dictionary<CultureInfo, string>()
        {
            { Language.Hebrew.Culture,"63d3a53b-1836-403e-9a29-90e937c33616"},
            {Language.English.Culture ,"73ce9f9a-990e-4dd7-b3f5-108a961b8464" }
        };
    }
}