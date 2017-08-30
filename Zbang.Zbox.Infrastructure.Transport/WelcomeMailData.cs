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
            : base(emailAddress, culture)
        {
            UserName = userName;
        }
        [ProtoMember(1)]
        public string UserName { get; set; }


        public override string MailResolver => WelcomeResolver;
    }

    [ProtoContract]
    public class NewUserData : DomainProcess
    {
        protected NewUserData()
        {
        }
        public NewUserData(long userId, string userName, string culture, string emailAddress, string referrel)
        {
            UserName = userName;
            Culture = culture;
            EmailAddress = emailAddress;
            Referrel = referrel;
            UserId = userId;
        }

        [ProtoMember(1)]
        public string UserName { get; set; }
        [ProtoMember(2)]
        public string Culture { get; set; }

        [ProtoMember(3)]
        public string EmailAddress { get; set; }

        [ProtoMember(4)]
        public string Referrel { get; set; }
        [ProtoMember(5)]
        public long UserId { get; set; }
        public override string ProcessResolver => UserResolver;
    }
}
