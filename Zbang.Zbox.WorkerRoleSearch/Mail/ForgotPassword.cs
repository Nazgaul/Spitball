using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal class ForgotPassword : IMail2
    {
        private readonly IMailComponent _mailComponent;
        public ForgotPassword(IMailComponent mailComponent)
        {
            _mailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is ForgotPasswordData2 parameters2)) return false;
            await _mailComponent.DeleteUnsubscribeAsync(parameters2.EmailAddress).ConfigureAwait(false);
            await _mailComponent.GenerateAndSendEmailAsync(parameters2.EmailAddress,
                new ForgotPasswordMailParams2(parameters2.Code,
                    string.Format(UrlConst.PasswordUpdate, parameters2.Link), parameters2.Name,
                    new CultureInfo(parameters2.Culture)), token).ConfigureAwait(false);
           return true;
        }
    }
}
