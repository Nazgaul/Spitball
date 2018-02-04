
namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class DepartmentApprovedAccessMail : MailBuilder
    {
        private const string Category = "Request approved";
        private const string Subject = "Approved access";

        private readonly DepartmentApprovedMailParams _parameters;

        public DepartmentApprovedAccessMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as DepartmentApprovedMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.DepartmentApprovedAccess");
            return html.Replace("{DEP_NAME}", _parameters.DepName);
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