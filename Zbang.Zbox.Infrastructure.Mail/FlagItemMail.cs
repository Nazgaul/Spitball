
namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class FlagItemMail : MailBuilder
    {
        private const string Category = "Flag Bad Item";
        private const string Subject = "Flag Bad Item";

        private readonly FlagItemMailParams m_Parameters;

        public FlagItemMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as FlagItemMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(m_Parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.FlaggedItem");

            html = html.Replace("{ItemUrl}", m_Parameters.Url);
            html = html.Replace("{ITEM NAME}", m_Parameters.ItemName);
            html = html.Replace("{REASON}", m_Parameters.Reason);
            html = html.Replace("{FLAGGER-USERNAME}", m_Parameters.UserName);
            html = html.Replace("{FLAGGER-EMAIL}", m_Parameters.Email);
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
