namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class ForgotPasswordMail : MailBuilder
    {
        private const string Category = "Password Recovery";
        private const string Subject = "Password Recovery";

        private readonly ForgotPasswordMailParams2 _parameters;

        public ForgotPasswordMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as ForgotPasswordMailParams2;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.ResetPwd");
            html = html.Replace("{NEW-PWD}", _parameters.Code);
            html = html.Replace("{CHANGE-URL}", _parameters.Link);
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
