using System;

namespace Cloudents.Core.Storage
{
    [Serializable]
    public sealed class QueueName
    {
        public const string UrlRedirectName = "url-redirect2";
       // public const string CommunicationName = "communication";
       // public const string BackgroundName = "communication";
        //public const string SmsName = "sms";
       // public const string EmailName = "email";
        //public string Key { get; }

        //private QueueName(string key)
        //{
        //    Key = key;
        //}

        //public static readonly QueueName UrlRedirect = new QueueName(UrlRedirectName);
        
        //public static readonly QueueName Background = new QueueName(BackgroundName);
        //public static readonly QueueName Email = new QueueName(EmailName);
    }

    

    

    [Serializable]
    public abstract class BaseEmail
    {
        protected BaseEmail(string to, string template, string subject)
        {
            To = to;
            Template = template;
            Subject = subject;
        }

        public string To { get; private set; }

        public string Template { get; private set; }
        public string Subject { get; private set; }
    }

    public abstract class QueueBackground
    {

    }
    
}