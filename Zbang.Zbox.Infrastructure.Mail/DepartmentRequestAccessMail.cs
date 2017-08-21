namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class DepartmentRequestAccessMail : MailBuilder
    {
        private const string Category = "Request access";
        private const string Subject = "Another spitballer requests access";

        private readonly DepartmentRequestAccessMailParams m_Parameters;

        public DepartmentRequestAccessMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as DepartmentRequestAccessMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(m_Parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.RequestDepartmentAccess");
            
            html = html.Replace("{USER_NAME}", m_Parameters.UserName);
            html = html.Replace("{USER_IMG}", m_Parameters.UserImage);
            html = html.Replace("{DEP_NAME}", m_Parameters.DepName);
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