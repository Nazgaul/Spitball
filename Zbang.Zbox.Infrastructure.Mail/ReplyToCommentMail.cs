using System;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class ReplyToCommentMail : MailBuilder
    {
        private readonly ReplyToCommentMailParams m_Parameters;

        public ReplyToCommentMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as ReplyToCommentMailParams;
        }
        public override string GenerateMail()
        {
            if (m_Parameters == null) throw new ArgumentNullException(nameof(m_Parameters));
            var html = LoadMailTempate.LoadMailFromContent(m_Parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ReplyToCommentMailTemplate");
            html = html.Replace("{name}", m_Parameters.UserName);
            html = html.Replace("{UserWhoMadeActionName}", m_Parameters.UserWhoMadeActionName);
            html = html.Replace("{BoxName}", m_Parameters.BoxName);
            html = html.Replace("{BoxUrl}", UrlConst.AppendCloudentsUrl(m_Parameters.BoxUrl));
            return html;
        }

        public override string AddSubject()
        {
            return EmailResource.ReplyToCommentSubject;
        }

        public override string AddCategory()
        {
            return "replyToComment";
        }
    }
}
