using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class ReplyToCommentData : BaseMailData
    {
        protected ReplyToCommentData()
        {

        }
        public ReplyToCommentData(string emailAddress, string culture, string userName, string userWhoMadeAction, string boxOrItemName, string boxOrItemUrl)
            : base(emailAddress, culture)
        {
            UserName = userName;
            UserWhoMadeAction = userWhoMadeAction;
            BoxOrItemName = boxOrItemName;
            BoxOrItemUrl = boxOrItemUrl;
        }
        [ProtoMember(1)]
        public string UserName { get; private set; }
        [ProtoMember(2)]
        public string UserWhoMadeAction { get; private set; }
        [ProtoMember(3)]
        public string BoxOrItemName { get; private set; }
        [ProtoMember(4)]
        public string BoxOrItemUrl { get; private set; }
        public override string MailResolver => nameof(ReplyToCommentData);

        public override string ToString()
        {
            var str = 
                $"UserName: {UserName} UserWhoMadeAction {UserWhoMadeAction} BoxOrItemName {BoxOrItemName} BoxOrItemUrl {BoxOrItemUrl}";
            var str2 = base.ToString();
            return str + str2;
        }
    }
}
