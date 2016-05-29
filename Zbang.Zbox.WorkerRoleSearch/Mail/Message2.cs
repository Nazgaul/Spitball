using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal class Message2 : IMail2
    {
        private readonly IMailComponent m_MailComponent;
        public Message2(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }
        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            var parameters2 = data as MessageMailData2;

            if (parameters2 == null) return true;
            var userImage = parameters2.SenderUserImage ?? "https://az32006.vo.msecnd.net/zboxprofilepic/DefaultEmailImage.jpg";
            await m_MailComponent.GenerateAndSendEmailAsync(parameters2.EmailAddress,
                  new MessageMailParams(parameters2.Message, parameters2.SenderUserName,
                      new CultureInfo(parameters2.Culture), parameters2.SenderUserEmail, userImage), token);

            return true;

        }
    }
}
