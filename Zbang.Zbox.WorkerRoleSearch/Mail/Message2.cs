using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal class Message2 : IMail2
    {
        private readonly IMailComponent _mailComponent;
        public Message2(IMailComponent mailComponent)
        {
            _mailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is MessageMailData parameters)) return true;
            var userImage = parameters.SenderUserImage ?? "https://az32006.vo.msecnd.net/zboxprofilepic/DefaultEmailImage.jpg";
            await _mailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                  new MessageMailParams(parameters.Message, parameters.SenderUserName,
                      new CultureInfo(parameters.Culture), parameters.SenderUserEmail, userImage), token);
            return true;
        }

    }
}
