using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    public class LibraryAccessApproved : IMail2
    {
        private readonly IMailComponent m_MailComponent;

        public LibraryAccessApproved(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }

        public async Task<bool> ExecuteAsync(BaseMailData data)
        {
            var parameters = data as AccessApprovedData;
            if (parameters == null)
            {
                throw new NullReferenceException("parameters");
            }
            string depName = parameters.DepName;

            await m_MailComponent.GenerateAndSendEmailAsync(parameters.EmailAddress,
                 new DepartmentApprovedMailParams(new CultureInfo(parameters.Culture), depName));

            return true;
        }
    }
}
