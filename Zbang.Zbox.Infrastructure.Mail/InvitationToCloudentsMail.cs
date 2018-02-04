namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class InvitationToCloudentsMail : MailBuilder
    {
        private const string Category = "InvitationCloudents";
        private const string Subject = "Invite to Spitball";

        private readonly InvitationToCloudentsMailParams _parameters;

        public InvitationToCloudentsMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as InvitationToCloudentsMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.InviteCloudents");

            html = html.Replace("{USERNAME}", _parameters.SenderName);
            html = html.Replace("{Image}", _parameters.SenderImage);
            html = html.Replace("{{Url}}", _parameters.Url);
            return html;
        }

        public override string AddSubject()
        {
            return Subject;
        }

        public override string AddCategory()
        {
            return Category;
        }
    }
}
