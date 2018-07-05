using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Storage
{
    [Serializable]
    public class RegistrationEmail : BaseEmail
    {
        public RegistrationEmail(string to, string link) : base(to, "register","Welcome to Spitball")
        {
            Link = link;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Using it with reflection")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Using it with reflection")]
        public string Link { get; private set; }
    }

    [Serializable]
    public class GotAnswerEmail: BaseEmail
    {
        public GotAnswerEmail(string questionText ,string to) : base(to, null, "You got an answer to question you asked")
        {
            QuestionText = questionText;
        }

        public string QuestionText { get;private set; }

        public override string ToString()
        {
            return $"You got an answer to question: {QuestionText}";
        }
    }


    [Serializable]
    public class AnswerCorrectEmail : BaseEmail
    {
        public AnswerCorrectEmail(string to) : base(to, null, "Congratz - answer correct")
        {
        }

        public override string ToString()
        {
            return $"Congratz one of your answers is correct";
        }
    }
}