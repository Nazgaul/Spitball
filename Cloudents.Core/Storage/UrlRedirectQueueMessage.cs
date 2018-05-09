using System;
using System.Runtime.Serialization;

namespace Cloudents.Core.Storage
{
    [DataContract]
    public class UrlRedirectQueueMessage : IQueueName
    {
        public UrlRedirectQueueMessage(string host,
            string url, string urlReferrer, int? location, string ip)
        {
            Host = host;
            Url = url;
            DateTime = DateTime.UtcNow;
            UrlReferrer = urlReferrer;
            Location = location;
            Ip = ip;
        }

        protected UrlRedirectQueueMessage()
        {
        }

        [DataMember(Order = 1)]
        public string Host { get; set; }
        [DataMember(Order = 2)]
        public string Url { get; set; }

        [DataMember(Order = 3)]
        public DateTime DateTime { get; set; }

        [DataMember(Order = 4)]
        public string UrlReferrer { get; set; }

        [DataMember(Order = 5)]
        public int? Location { get; set; }

        [DataMember(Order = 6)]
        public string Ip { get; set; }

        public QueueName QueueName => QueueName.UrlRedirect;
    }


    [DataContract]
    public class SmsMessage : IQueueName
    {
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string Message { get; set; }
        public QueueName QueueName => QueueName.Sms;
    }

    [DataContract]
    public class EmailMessage : IQueueName
    {
        [DataMember]
        public string To { get; set; }
        [DataMember]
        public string Template { get; set; }

        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public object[] PlaceHolders { get; set; }
        public QueueName QueueName => QueueName.Email;
    }
}
