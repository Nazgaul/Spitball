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

        public string UserName { get; }
        public string UserImage { get; }
        public string DepName { get; }

        public override string MailResolver => DepartmentRequestAccessResolver;
    }
}