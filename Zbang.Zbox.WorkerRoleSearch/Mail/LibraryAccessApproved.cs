using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    public class LibraryAccessApproved : IMail2
    {
        private readonly IMailComponent m_MailComponent;

        public LibraryAccessApproved(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token)
        {
            if (!(data is AccessApprovedData parameters))
            {
                throw new NullReferenceException("parameters");
            }
            //string depName = parameters.DepName;

            await m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                new DepartmentApprovedMailParams(new CultureInfo(parameters.Culture), parameters.DepName), token).ConfigureAwait(false);

            return true;
        }
    }
}
