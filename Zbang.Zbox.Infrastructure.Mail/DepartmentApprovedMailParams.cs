using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class DepartmentApprovedMailParams : MailParameters
    {
        public DepartmentApprovedMailParams(CultureInfo culture, string depName)
            : base(culture)
        {
            DepName = depName;
        }

        public string DepName { get; }
        public override string MailResolver => DepartmentRequestApprovedResolver;
    }
}