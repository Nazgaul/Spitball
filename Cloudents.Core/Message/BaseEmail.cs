using System;
using System.Diagnostics;

namespace Cloudents.Core.Message
{
    [Serializable]
    public abstract class BaseEmail
    {
        protected BaseEmail(string to, string template, string subject)
        {
            To = to;
            Template = template;
            Subject = subject;
        }

        [DebuggerDisplay("To = {To}")]

        public string To { get; private set; }

        public string Template { get; private set; }
        public string Subject { get; private set; }
    }
}