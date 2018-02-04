using Zbang.Zbox.Infrastructure.Mail.Resources;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class WelcomeMail : MailBuilder
    {
        private const string Category = "Welcome";

        private readonly WelcomeMailParams _parameters;

        public WelcomeMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as WelcomeMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.Welcome");
            return html.Replace("{Name}", _parameters.Name);
        }

        public override string AddSubject()
        {
            return EmailResource.WelcomeSubject;
        }

        public override string AddCategory()
        {
            return Category;
        }
    }
}
