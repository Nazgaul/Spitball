using SendGrid;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public abstract class MailBuilder : IMailBuilder
    {
        protected MailBuilder(MailParameters parameters)
        {

        }

        public abstract string GenerateMail();

        public abstract string AddSubject();

        public abstract string AddCategory();
    }

    public interface IMailBuilder
    {
        string GenerateMail();

        string AddSubject();
        string AddCategory();
    }
}
