using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class DepartmentRequestAccessMailParams : MailParameters
    {
        public DepartmentRequestAccessMailParams( CultureInfo culture)
            : base(culture)
        {
        }

        public override string MailResover
        {
            get { return DepartmentRequestAccessResolver; }
        }
    }
}