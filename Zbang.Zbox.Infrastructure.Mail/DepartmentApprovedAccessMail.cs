using System;
using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class DepartmentApprovedAccessMail : MailBuilder
    {
        private const string Category = "Request approved";
        private const string Subject = "Approved access";

        private readonly DepartmentApprovedMailParams m_Parameters;

        public DepartmentApprovedAccessMail(MailParameters parameters) : base(parameters)
        {
            m_Parameters = parameters as DepartmentApprovedMailParams;
        }
        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(m_Parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.DepartmentApprovedAccess");
            return html.Replace("{DEP_NAME}", m_Parameters.DepName);
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