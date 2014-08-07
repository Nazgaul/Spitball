using System.Globalization;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class MessageMailParams : MailParameters
    {
        public MessageMailParams(string message, string senderUserName, CultureInfo culture)
            : base(culture, DefaultEmail, senderUserName)
        {
            Message = message;
            SenderImage = "https://zboxstorage.blob.core.windows.net/zboxprofilepic/S50X50/userpic9.jpg";
            //SenderUserName = senderUserName;
        }
        public MessageMailParams(string message, string senderUserName, CultureInfo culture, string senderEmail, string senderImage)
            : base(culture, senderEmail, senderUserName)
        {
            Message = message;
            SenderImage = senderImage;
        }
        public string Message { get; private set; }
        //public string SenderUserName { get; private set; }
        public string SenderImage { get; private set; }
        //public string SenderEmail { get; private set; }
        public override string MailResover
        {
            get { return MessageResolver; }
        }
    }
}