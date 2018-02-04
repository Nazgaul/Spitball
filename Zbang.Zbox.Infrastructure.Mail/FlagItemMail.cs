
namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class FlagItemMail : MailBuilder
    {
        private const string Category = "Flag Bad Item";
        private const string Subject = "Flag Bad Item";

        private readonly FlagItemMailParams _parameters;

        public FlagItemMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as FlagItemMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.FlaggedItem");

            html = html.Replace("{ItemUrl}", _parameters.Url);
            html = html.Replace("{ITEM NAME}", _parameters.ItemName);
            html = html.Replace("{REASON}", _parameters.Reason);
            html = html.Replace("{FLAGGER-USERNAME}", _parameters.UserName);
            html = html.Replace("{FLAGGER-EMAIL}", _parameters.Email);
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
