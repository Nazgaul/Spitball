using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class InvitationToCloudentsMailParams : MailParameters
    {
        public InvitationToCloudentsMailParams(string senderName, string senderImage, CultureInfo culture)
            : base(culture)
        {
            SenderName = senderName;
            SenderImage = senderImage;
        }

        public InvitationToCloudentsMailParams(string senderName, string senderImage, CultureInfo culture, string senderEmail)
            : base(culture, senderEmail, senderName)
        {
            SenderName = senderName;
            SenderImage = senderImage;
        }
        
        public new string SenderName { get; private set; }
        public string SenderImage { get; private set; }

        public override string MailResover
        {
            get { return InvitationToCloudentsResolver; }
        }
    }
}