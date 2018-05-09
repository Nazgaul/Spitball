using System;

namespace Cloudents.Core.Storage
{
    [Serializable]
    public sealed class QueueName
    {
        public const string UrlRedirectName = "url-redirect2";
        public const string SmsName = "sms";
        public const string EmailName = "email";
        public string Key { get; }

        private QueueName(string key)
        {
            Key = key;
        }

        public static readonly QueueName UrlRedirect = new QueueName(UrlRedirectName);
        public static readonly QueueName Sms = new QueueName(SmsName);
        public static readonly QueueName Email = new QueueName(EmailName);
    }

    public interface IQueueName
    {
        QueueName QueueName { get; }
    }
}