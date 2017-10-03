using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class RequestAccess : IMail2
    {
        private readonly IMailComponent m_MailComponent;

        public RequestAccess(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is RequestAccessData parameters))
            {
                throw new NullReferenceException("parameters");
            }
            //string userName =parameters.UserName;
            //string userImage =parameters.UserImage;
            //string depName = parameters.DepName;

            await m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                new DepartmentRequestAccessMailParams(new CultureInfo(parameters.Culture),
                    parameters.UserName, parameters.UserImage, parameters.DepName), token).ConfigureAwait(false);

            return true;
        }
    }
}
