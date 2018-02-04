namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class DepartmentRequestAccessMail : MailBuilder
    {
        private const string Category = "Request access";
        private const string Subject = "Another spitballer requests access";

        private readonly DepartmentRequestAccessMailParams _parameters;

        public DepartmentRequestAccessMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as DepartmentRequestAccessMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.RequestDepartmentAccess");
            
            html = html.Replace("{USER_NAME}", _parameters.UserName);
            html = html.Replace("{USER_IMG}", _parameters.UserImage);
            html = html.Replace("{DEP_NAME}", _parameters.DepName);
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