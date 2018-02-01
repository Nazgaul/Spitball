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
        private readonly IMailComponent m_MailComponent;
        private readonly IIntercomApiManager m_IntercomManager;

        public NewUserProcess(IMailComponent mailComponent, IIntercomApiManager intercomManager)
        {
            m_MailComponent = mailComponent;
            m_IntercomManager = intercomManager;
        }

        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            var parameters = data as NewUserData;
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            var t1 = Task.CompletedTask;
            if (!string.IsNullOrEmpty(parameters.Referrel))
            {
              // t1 =  m_IntercomManager.UpdateUserRefAsync(parameters.UserId, parameters.EmailAddress, parameters.Referrel, token);
            }

            var t2 =  m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                 new WelcomeMailParams(parameters.UserName,
                     new CultureInfo(parameters.Culture)), token);
            await Task.WhenAll(t1, t2).ConfigureAwait(false);
            return true;
        }
    }
}
