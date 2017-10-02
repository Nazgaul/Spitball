using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class InvitationToCloudentsMailParams : MailParameters
    {
       public InvitationToCloudentsMailParams(string senderName, string senderImage, CultureInfo culture, string senderEmail, string url)
            : base(culture, senderEmail, senderName)
        {
            Url = url;
            SenderName = senderName;
            SenderImage = senderImage;
        }
        public new string SenderName { get; }
        public string SenderImage { get; }

        public override string MailResolver => InvitationToCloudentsResolver;

        public string Url { get; }
    }
}