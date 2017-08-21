﻿using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal class ChangeEmail : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        public ChangeEmail(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            var parameters = data as ChangeEmailData;
            if (parameters == null)
            {
                throw new System.NullReferenceException("parameters");
            }

            await m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                 new ChangeEmailMailParams(parameters.Code,
                new CultureInfo(parameters.Culture)), token);

            return true;
        }
    }
}
