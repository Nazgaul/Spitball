using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Routes;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class Message : IMail
    {
        readonly IMailManager m_MailManager;

        public Message(IMailManager mailManager)
        {
            m_MailManager = mailManager;
        }

        public bool Excecute(MailQueueData data)
        {
            var messageData = data as MessageMail;
            if (messageData == null)
            {
                throw new ArgumentException("Message mail did not receive proper stream");
            }
            foreach (var invitedEmail in messageData.To)
            {
                m_MailManager.SendPersonalEmail(
                    new Infrastructure.Mail.Parameters.Message(messageData.SenderEmail,
                                                            messageData.SenderName,
                                                            invitedEmail,
                                                            messageData.PersonalNote,
                                                            messageData.Url
                                                            ));
            }
            return true;
        }
    }
}
