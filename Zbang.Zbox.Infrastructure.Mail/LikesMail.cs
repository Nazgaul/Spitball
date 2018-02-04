using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class LikesMail : MailBuilder
    {
        private readonly LikesMailParams _parameters;

        public LikesMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as LikesMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContentWithDot(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.MarketingTemplate");
            html.Replace("{name}", _parameters.Name);

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(string.Format(EmailResource.LikesMailTitle, _parameters.LikeData.Count()));
            foreach (var likeData in _parameters.LikeData)
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
