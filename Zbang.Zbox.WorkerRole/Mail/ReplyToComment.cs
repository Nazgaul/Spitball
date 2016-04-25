﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    public class ReplyToComment : IMail2
    {
        private readonly IMailComponent m_MailComponent;

        public ReplyToComment(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }
        public async Task<bool> ExecuteAsync(BaseMailData data)
        {
            var parameters = data as ReplyToCommentData;
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            await m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                 new ReplyToCommentMailParams(new CultureInfo(parameters.Culture),parameters.UserName,parameters.UserWhoMadeAction,parameters.BoxOrItemName,parameters.BoxOrItemUrl));

            return true;
        }
    }
}
