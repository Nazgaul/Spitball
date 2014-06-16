using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public interface IMailBuilder
    {
        void GenerateMail(ISendGrid message, MailParameters parameters);
    }
}
