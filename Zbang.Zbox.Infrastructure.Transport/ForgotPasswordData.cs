using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{

    [ProtoContract]
    public class ForgotPasswordData2 : BaseMailData
    {
        protected ForgotPasswordData2()
        {

        }
        public ForgotPasswordData2(string code, string linkAddress,string name, string emailAddress, string culture)
            : base(emailAddress, culture)
        {
            Code = code;
            Link = linkAddress;
            Name = name;
        }
        [ProtoMember(2)]
        public string Code { get; private set; }

        [ProtoMember(3)]
        public string Link { get; private set; }

        [ProtoMember(4)]
        public string Name { get; private set; }

        public override string MailResover
        {
            get { return ForgotPasswordResolver; }
        }
    }
}
