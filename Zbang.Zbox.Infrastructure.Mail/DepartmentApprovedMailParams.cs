using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class DepartmentApprovedMailParams : MailParameters
    {
        public DepartmentApprovedMailParams(CultureInfo culture)
            : base(culture)
        {
        }

        public override string MailResover
        {
            get { return DepartmentRequestApprovedResolver; }
        }
    }
}