using System;
using System.Globalization;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class InviteToCloudents : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        public InviteToCloudents(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }

        public Task<bool> ExecuteAsync(BaseMailData data)
        {
            var parameters = data as InviteToCloudentsData;

            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            var userImage = parameters.SenderImage ?? "https://az32006.vo.msecnd.net/zboxprofilepic/DefaultEmailImage.jpg";

            var inviterEmail = parameters.SenderEmail;
            if (inviterEmail.Contains("yahoo"))
            {
                inviterEmail = MailParameters.DefaultEmail;
            }
            var url = string.IsNullOrEmpty(parameters.Url) ? "https://www.cloudents.com" : parameters.Url;
            m_MailComponent.GenerateAndSendEmail(parameters.EmailAddress,
             new InvitationToCloudentsMailParams(parameters.SenderName, userImage,
            new CultureInfo(parameters.Culture), inviterEmail, url));

            return Task.FromResult(true);
        }
    }
}
