using System;

namespace Cloudents.Core.Storage
{
    [Serializable]
    public sealed class QueueName
    {
        public const string UrlRedirectName = "url-redirect";
        public string Key { get; }

        private QueueName(string key)
        {
            Key = key;
        }

        public static readonly QueueName UrlRedirect = new QueueName(UrlRedirectName);
    }
}