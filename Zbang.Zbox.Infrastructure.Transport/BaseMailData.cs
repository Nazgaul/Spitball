using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    [ProtoInclude(10, typeof(WelcomeMailData))]
    [ProtoInclude(11, typeof(InviteMailData))]

    //[ProtoInclude(13, typeof(MessageMailData))]
    
    [ProtoInclude(15, typeof(ChangeEmailData))]
    [ProtoInclude(16, typeof(ForgotPasswordData2))]
    [ProtoInclude(17, typeof(InviteToCloudentsData))]
    [ProtoInclude(18, typeof(MessageMailData))]
    [ProtoInclude(19, typeof(RequestAccessData))]
    [ProtoInclude(20, typeof(AccessApprovedData))]
    [ProtoInclude(21, typeof(ReplyToCommentData))]
    public abstract class BaseMailData
    {
        public const string WelcomeResolver = "welcome";
        public const string InviteResolver = "invite";
        public const string ForgotPasswordResolver = "forgot";
        public const string MessageResolver = "message";
        public const string ChangeEmailResolver = "ChangeEmail";
        public const string InviteToCloudentsResolver = "inviteToCloudents";
        public const string RequestAccessResolver = "requestAccessResolver";
        public const string AccessApprovedResolver = "accessApprovedResolver";

        protected BaseMailData()
        {
        }

        protected BaseMailData(string emailAddress,string culture)
        {
            Culture = culture;
            EmailAddress = emailAddress;
        }
        [ProtoMember(1)]
        public string Culture { get; set; }

        [ProtoMember(2)]
        public string EmailAddress { get; private set; }

        public abstract string MailResolver { get; }

        public override string ToString()
        {
            return $"Culture {Culture} EmailAddress {EmailAddress}";
        }
    }
}
