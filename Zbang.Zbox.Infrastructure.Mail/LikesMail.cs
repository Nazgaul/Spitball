using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class LikesMail : MailBuilder
    {
        private readonly LikesMailParams m_Parameters;

        public LikesMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as LikesMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContentWithDot(m_Parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.MarketingTemplate");
            html.Replace("{name}", m_Parameters.Name);

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(string.Format(EmailResource.LikesMailTitle, m_Parameters.LikeData.Count()));
            foreach (var likeData in m_Parameters.LikeData)
            {
                sb.AppendLine(string.Format(EmailResource.LikesMailLineDetails, likeData.UserName, likeData.Type.GetEnumDescription(), likeData.OnLikeText));
            }
            sb.AppendLine();
            sb.AppendLine(EmailResource.LikesMailFooter);
            html.Replace("{body}", sb.ToString().Replace("\n", "<br>"));
            return html.ToString();
        }

        public override string AddSubject()
        {
            return EmailResource.LikesMailSubject;
        }

        public override string AddCategory()
        {
            return "like";
        }
    }
}
