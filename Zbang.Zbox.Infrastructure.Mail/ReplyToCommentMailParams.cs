using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class ReplyToCommentMailParams : MailParameters
    {
        public ReplyToCommentMailParams(CultureInfo culture, string userName, string userWhoMadeActionName,
            string boxName, string boxUrl)
            : base(culture)
        {
            UserName = userName;
            UserWhoMadeActionName = userWhoMadeActionName;
            BoxName = boxName;
            BoxUrl = boxUrl;
        }

        public string UserName { get; private set; }
        public string UserWhoMadeActionName { get; private set; }
        public string BoxName { get; private set; }
        public string BoxUrl { get; private set; }

        public override string MailResover => nameof(ReplyToCommentMailParams);
    }
}