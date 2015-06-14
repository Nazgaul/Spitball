using System.Globalization;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class ChangeEmail : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        public ChangeEmail(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }
        public async Task<bool> ExecuteAsync(BaseMailData data)
        {
            var parameters = data as ChangeEmailData;
            if (parameters == null)
            {
                throw new System.NullReferenceException("parameters");
            }

            await m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                 new ChangeEmailMailParams(parameters.Code,
                new CultureInfo(parameters.Culture)));

            return true;

        }
    }
}
