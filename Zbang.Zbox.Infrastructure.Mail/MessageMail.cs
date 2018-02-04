namespace Zbang.Zbox.Infrastructure.Mail
{
    internal class MessageMail : MailBuilder
    {
        private const string Category = "Personal Message";
        private const string Subject = "Message via Spitball from {0}";

        private readonly MessageMailParams _parameters;

        public MessageMail(MailParameters parameters) : base(parameters)
        {
            _parameters = parameters as MessageMailParams;
        }

        public override string GenerateMail()
        {
            var html = LoadMailTempate.LoadMailFromContent(_parameters.UserCulture, "Zbang.Zbox.Infrastructure.Mail.MailTemplate.PersonalMsg");
            html = html.Replace("{USER_MSG}", _parameters.Message);
            html = html.Replace("{MSG_AUTHOR}", _parameters.SenderName);
            html = html.Replace("{ImgUrl}", _parameters.SenderImage);
            //sb.Replace("{REPLY_URL}", messageParams.SenderEmail);

            return html;
        }

        public override string AddCategory()
        {
            return Category;
        }

        public override string AddSubject()
        {
            return string.Format(Subject, _parameters.SenderName);
        }
    }
}
