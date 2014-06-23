using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal class ForgotPassword : Imail2
    {
        private readonly IMailComponent m_MailComponent;
        public ForgotPassword(IMailComponent mailComponent)
        {
            m_MailComponent = mailComponent;
        }
        public bool Excecute(BaseMailData data)
        {
            var parameters2 = data as ForgotPasswordData2;
            if (parameters2 == null) return false;
            var t = m_MailComponent.DeleteUnsubscribe(parameters2.EmailAddress);
            t.Wait();
            m_MailComponent.GenerateAndSendEmail(parameters2.EmailAddress,
                new ForgotPasswordMailParams2(parameters2.Code, string.Format(UrlConsts.PasswordUpdate, parameters2.Link), parameters2.Name,
                    new CultureInfo(parameters2.Culture)));
            return true;
        }
    }
}
