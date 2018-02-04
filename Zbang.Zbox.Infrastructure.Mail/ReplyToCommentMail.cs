using System;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class ReplyToCommentMail : MailBuilder
    {
        private readonly ReplyToCommentMailParams _parameters;

        public ReplyToCommentMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as ReplyToCommentMailParams;
        }

        public override string GenerateMail()
        {
            if (_parameters == null) throw new ArgumentNullException(nameof(_parameters));
            var html = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ReplyToCommentMailTemplate");
            html = html.Replace("{name}", _parameters.UserName);
            html = html.Replace("{UserWhoMadeActionName}", _parameters.UserWhoMadeActionName);
            html = html.Replace("{BoxName}", _parameters.BoxName);
            html = html.Replace("{BoxUrl}", UrlConst.AppendCloudentsUrl(_parameters.BoxUrl));
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
