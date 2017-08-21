using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class DepartmentRequestAccessMailParams : MailParameters
    {
        public DepartmentRequestAccessMailParams(CultureInfo culture, string userName, string userImage, string depName)
            : base(culture)
        {
            DepName = depName;
            UserImage = userImage;
            UserName = userName;
        }

        public string UserName { get; private set; }
        public string UserImage { get; private set; }
        public string DepName { get; private set; }

        public override string MailResover
        {
            get { return DepartmentRequestAccessResolver; }
        }
    }
}