using System;
using System.Diagnostics;

namespace Cloudents.Core.Message
{
    [Serializable]
    public abstract class BaseEmail
    {
        protected BaseEmail(string to, string template, string subject,string source = null, string medium = null, 
            string campaign = null)
        {
            To = to;
            Template = template;
            Subject = subject;
            Source = source;
            Medium = medium;
            Campaign = campaign;
        }

        [DebuggerDisplay("To = {To}")]

        public string To { get; private set; }

        public string Template { get; private set; }
        public string Subject { get; private set; }
        public string Source { get; private set; }
        public string Medium { get; private set; }
        public string Campaign { get; private set; }

    }
}