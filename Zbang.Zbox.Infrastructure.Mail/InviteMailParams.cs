using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class InviteMailParams : MailParameters
    {
        public InviteMailParams(string invitor, string boxname, string boxurl, string invitorImage, CultureInfo culture)
            : base(culture)
        {
            Invitor = invitor;
            BoxName = boxname;
            BoxUrl = boxurl;
            InvitorImage = invitorImage;
        }

        public InviteMailParams(string invitor, string boxname, string boxurl, string invitorImage, CultureInfo culture, string senderEmail)
            : base(culture, senderEmail, invitor)
        {
            Invitor = invitor;
            BoxName = boxname;
            BoxUrl = boxurl;
            InvitorImage = invitorImage;
        }

        public string Invitor { get; }
        public string BoxName { get; }
        public string BoxUrl { get; }
        public string InvitorImage { get; set; }
        public override string MailResolver => InviteResolver;
    }
}