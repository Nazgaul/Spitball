using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class RequestAccessData : BaseMailData
    {
        protected RequestAccessData()
        {

        }
        public RequestAccessData(string emailAddress, string culture)
            : base(emailAddress, culture)
        {
        }
        public override string MailResover
        {
            get { return RequestAccessResolver; }
        }
    }
}
