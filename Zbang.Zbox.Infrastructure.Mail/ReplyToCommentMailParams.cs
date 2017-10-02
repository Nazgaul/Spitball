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

        public string UserName { get; }
        public string UserWhoMadeActionName { get; }
        public string BoxName { get; }
        public string BoxUrl { get; }

        public override string MailResolver => nameof(ReplyToCommentMailParams);
    }
}