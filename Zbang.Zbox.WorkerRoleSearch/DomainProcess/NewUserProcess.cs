using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    public class NewUserProcess : IDomainProcess
    {
        private readonly IMailComponent _mailComponent;

        public NewUserProcess(IMailComponent mailComponent)
        {
            _mailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            if (!(data is NewUserData parameters))
            {
                throw new NullReferenceException("parameters");
            }
            var t1 = Task.CompletedTask;
            if (!string.IsNullOrEmpty(parameters.Referrel))
            {
              // t1 =  m_IntercomManager.UpdateUserRefAsync(parameters.UserId, parameters.EmailAddress, parameters.Referrel, token);
            }

            var t2 =  _mailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                 new WelcomeMailParams(parameters.UserName,
                     new CultureInfo(parameters.Culture)), token);
            await Task.WhenAll(t1, t2).ConfigureAwait(false);
            return true;
        }
    }
}
