using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Cloudents.Core.Request
{
    [DataContract]
    public class MailGunRequest
    {
        public MailGunRequest(string to, string subject,
            string html, string tag, int interVal)
        {
            To = to;
            Subject = subject;
            Html = html;
            Tag = tag;
            _deliveryTime = DateTime.UtcNow.AddMinutes(interVal);
        }

        private DateTime _deliveryTime;

        [DataMember(Name = "from")] public string From => "no-reply@Spitball.co";
        [DataMember(Name = "to")]
        public string To { get; set; }

        [DataMember(Name = "subject")]
        public string Subject { get; set; }
        [DataMember(Name = "html")]
        public string Html { get; set; }
        [DataMember(Name = "o:tag")]
        public string Tag { get; set; }

        [DataMember(Name = "o:campaign")] [UsedImplicitly]
        public string Campaign => "spamgun";

        [DataMember(Name = "o:deliverytime")]
        [UsedImplicitly]
        public string DeliverIn => _deliveryTime.ToString("ddd, dd MMM yyyy HH:mm:ss UTC");
    }
}