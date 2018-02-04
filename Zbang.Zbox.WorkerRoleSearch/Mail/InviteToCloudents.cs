using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal class InviteToCloudents : IMail2
    {
        private readonly IMailComponent _mailComponent;
        public InviteToCloudents(IMailComponent mailComponent)
        {
            _mailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is InviteToCloudentsData parameters))
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
            await _mailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                new InvitationToCloudentsMailParams(parameters.SenderName, userImage,
                    new CultureInfo(parameters.Culture), inviteeEmail, url), token).ConfigureAwait(false);

            return true;
        }
    }
}
