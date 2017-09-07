using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal class Welcome : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        public Welcome(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is WelcomeMailData parameters))
            {
                throw new NullReferenceException("parameters");
            }

           await m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
               new WelcomeMailParams(parameters.UserName,
                   new CultureInfo(parameters.Culture)), token).ConfigureAwait(false);

            return true;
        }
    }
}
