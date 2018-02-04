using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal class ChangeEmail : IMail2
    {
        private readonly IMailComponent _mailComponent;
        public ChangeEmail(IMailComponent mailComponent)
        {
            _mailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is ChangeEmailData parameters))
            {
                throw new System.NullReferenceException("parameters");
            }

            await _mailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                new ChangeEmailMailParams(parameters.Code,
                    new CultureInfo(parameters.Culture)), token).ConfigureAwait(false);

            return true;
        }
    }
}
