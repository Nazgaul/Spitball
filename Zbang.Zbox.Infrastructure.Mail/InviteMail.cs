using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class InviteMail : MailBuilder
    {
        private const string Category = "Invite";

        private readonly InviteMailParams _parameters;

        public InviteMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as InviteMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.InviteCourse");
            html = html.Replace("{INVITOR}", _parameters.Invitor);
            html = html.Replace("{BOXNAME}", _parameters.BoxName);
            html = html.Replace("{BoxUrl}", _parameters.BoxUrl);
            html = html.Replace("{ImgUrl}", _parameters.InvitorImage);
            return html;
        }

        public override string AddSubject()
        {
            return string.Format(EmailResource.InviteSubject, _parameters.BoxName);
        }

        public override string AddCategory()
        {
            return Category;
        }
    }
}
