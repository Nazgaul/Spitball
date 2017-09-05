using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class ReplyToComment : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        private readonly ILogger m_Logger;

        public ReplyToComment(IMailComponent mailComponent, ILogger logger)
        {
            m_MailComponent = mailComponent;
            m_Logger = logger;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            var parameters = data as ReplyToCommentData;
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            try
            {
                await m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                    new ReplyToCommentMailParams(new CultureInfo(parameters.Culture), parameters.UserName,
                        parameters.UserWhoMadeAction, parameters.BoxOrItemName, parameters.BoxOrItemUrl), token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex);
                return false;
            }

            return true;
        }
    }
}
