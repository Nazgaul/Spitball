using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class WelcomeMailData : BaseMailData
    {
        protected WelcomeMailData()
        {
        }
        public WelcomeMailData(string emailAddress, string userName, string culture)
            : base(emailAddress,culture)
        {
            UserName = userName;
        }
        [ProtoMember(1)]
        public string UserName { get; set; }


        public override string MailResover
        {
            get { return WelcomeResolver; }
        }
    }
}
