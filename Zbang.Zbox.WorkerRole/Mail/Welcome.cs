using System;
using System.Globalization;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class Welcome : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        public Welcome(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }
        public async Task<bool> ExecuteAsync(BaseMailData data)
        {
            var parameters = data as WelcomeMailData;
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }


           await m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                new WelcomeMailParams(parameters.UserName,
                    new CultureInfo(parameters.Culture)));

            return true;

        }
    }
}
