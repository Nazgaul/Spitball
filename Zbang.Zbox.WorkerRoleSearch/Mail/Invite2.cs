using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal class Invite2 : IMail2
    {
        private readonly IMailComponent _mailComponent;
        public Invite2(IMailComponent mailComponent)
        {
            _mailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is InviteMailData parameters))
            {
                throw new NullReferenceException(nameof(parameters));
            }
            var userImage = parameters.InviterImage ?? "https://az32006.vo.msecnd.net/zboxprofilepic/DefaultEmailImage.jpg";


            var inviteeEmail = parameters.InviterEmail;
            if (inviteeEmail.Contains("yahoo"))
            {
                inviteeEmail = MailParameters.DefaultEmail;
            }
            await _mailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                new InviteMailParams(parameters.InviterName,
                    parameters.BoxName,
                    parameters.BoxUrl,
                    userImage,
                    new CultureInfo(parameters.Culture), inviteeEmail), token).ConfigureAwait(false);

            return true;
        }
    }
}
