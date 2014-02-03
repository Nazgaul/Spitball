using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class ImediateNotificationData : BaseMailData
    {
        protected ImediateNotificationData()
        {

        }
        public ImediateNotificationData(string boxName, string itemName,string itemUrl, string emailAddress, string culture)
            :base(emailAddress,culture)
        {
            //BoxName = boxName;
            //ItemName = itemName;
            //ItemUrl = itemUrl;
        }

        [ProtoMember(1)]
        public string BoxId { get; private set; }
        //[ProtoMember(2)]
        //public string ItemName { get; private set; }
        //[ProtoMember(3)]
        //public string ItemUrl { get; private set; }


        public override string MailResover
        {
            get { return ImmediateResolver; }
        }
    }
}
