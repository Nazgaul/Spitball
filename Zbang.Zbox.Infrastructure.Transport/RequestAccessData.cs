using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class RequestAccessData : BaseMailData
    {
        public RequestAccessData(string emailAddress, string culture, string userName, string userImage, string depName)
            : base(emailAddress, culture)
        {
            UserName = userName;
            UserImage = userImage;
            DepName = depName;
        }
        protected RequestAccessData()
        {
        }

        [ProtoMember(1)]
        public string UserName { get; private set; }
        [ProtoMember(2)]
        public string UserImage { get; private set; }
        [ProtoMember(3)]
        public string DepName { get; private set; }


        public override string MailResolver => RequestAccessResolver;
    }
}
