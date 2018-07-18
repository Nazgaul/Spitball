using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Cloudents.Core.Request
{
    [DataContract]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification="We are going via reflection")]
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

        [DataMember(Name = "from")]
        public string From => "Olivia Williams <olivia@spitball.co>";

        [DataMember(Name = "to")]
        public string To { [UsedImplicitly] get; }

        [DataMember(Name = "subject")]
        public string Subject { [UsedImplicitly] get; }

        [DataMember(Name = "html")]
        public string Html { [UsedImplicitly]get;  }

        [DataMember(Name = "o:tag")]
        public string Tag { [UsedImplicitly] get;  }

        [DataMember(Name = "o:campaign")] [UsedImplicitly]
        public string Campaign => "spamgun";

        [DataMember(Name = "o:deliverytime")]
        [UsedImplicitly]
        public string DeliverIn => _deliveryTime.ToString("ddd, dd MMM yyyy HH:mm:ss UTC");
    }
}