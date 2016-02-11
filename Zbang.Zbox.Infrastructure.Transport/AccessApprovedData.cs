using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
     [ProtoContract]
    public class AccessApprovedData : BaseMailData
    {
         protected AccessApprovedData()
        {

        }
         public AccessApprovedData(string emailAddress, string culture)
            : base(emailAddress, culture)
        {
        }
        public override string MailResover
        {
            get { return AccessApprovedResolver; }
        }
    }
}
