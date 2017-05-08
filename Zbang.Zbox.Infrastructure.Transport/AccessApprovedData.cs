using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class AccessApprovedData : BaseMailData
    {
        public AccessApprovedData(string emailAddress, string culture, string depName)
            : base(emailAddress, culture)
        {
            DepName = depName;
        }

        protected AccessApprovedData()
        {
        }
        [ProtoMember(1)]
        public string DepName { get; private set; }

        public override string MailResolver => AccessApprovedResolver;
    }
}
