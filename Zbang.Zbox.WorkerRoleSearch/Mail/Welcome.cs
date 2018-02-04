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
        private readonly IMailComponent _mailComponent;
        public Welcome(IMailComponent mailComponent)
        {
            _mailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is WelcomeMailData parameters))
            {
                throw new NullReferenceException("parameters");
            }

           await _mailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
               new WelcomeMailParams(parameters.UserName,
                   new CultureInfo(parameters.Culture)), token).ConfigureAwait(false);

            return true;
        }
    }
}
