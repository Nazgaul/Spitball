﻿using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal class InviteToCloudents : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        public InviteToCloudents(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            var parameters = data as InviteToCloudentsData;

            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            var userImage = parameters.SenderImage ?? "https://az32006.vo.msecnd.net/zboxprofilepic/DefaultEmailImage.jpg";

            var inviteeEmail = parameters.SenderEmail;
            if (inviteeEmail.Contains("yahoo"))
            {
                inviteeEmail = MailParameters.DefaultEmail;
            }
            var url = string.IsNullOrEmpty(parameters.Url) ? "https://www.spitball.co/" : parameters.Url;
            await m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
              new InvitationToCloudentsMailParams(parameters.SenderName, userImage,
             new CultureInfo(parameters.Culture), inviteeEmail, url), token);

            return true;
        }
    }
}
