namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class ChangeEmailMail : MailBuilder
    {
        private const string Category = "Change Email";
        private const string Subject = "Change Email";

        private readonly ChangeEmailMailParams _parameters;

        public ChangeEmailMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as ChangeEmailMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ChangeEmail");
            return html.Replace("{CODE}", _parameters.Code);
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
