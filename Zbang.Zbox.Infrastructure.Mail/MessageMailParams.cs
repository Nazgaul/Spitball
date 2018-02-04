using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class MessageMailParams : MailParameters
    {
        public MessageMailParams(string message, string senderUserName, CultureInfo culture, string senderEmail, string senderImage)
            : base(culture, senderEmail, senderUserName)
        {
            Message = message;
            SenderImage = senderImage;
        }

        public string Message { get; }
        public string SenderImage { get; }
        public override string MailResolver => MessageResolver;
    }
}