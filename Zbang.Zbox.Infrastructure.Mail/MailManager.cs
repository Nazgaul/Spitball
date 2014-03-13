using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class MailManager2 : IMailComponent
    {
        //private readonly SmtpClient m_SmtpClient;

        //private readonly IUnityContainer m_Container;
        private readonly IocFactory m_Container = Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity;

        public MailManager2(/*IUnityContainer container*/)
        {
            // m_SmtpClient = new SmtpClient("smtp.sendgrid.net", 587);
            //m_SmtpClient.Credentials = new System.Net.NetworkCredential("cloudents", "zbangitnow");
            //m_SmtpClient.EnableSsl = true;

        }

        private void Send(SendGridMail.ISendGrid message)
        {
            try
            {

                var smtpClient = SendGridMail.Transport.SMTP.GetInstance(
                    new System.Net.NetworkCredential("cloudents", "zbangitnow"), "smtp.sendgrid.net", 587);
                //var smtpClient = SendGridMail.Transport.SMTP.GetInstance(
                //  new System.Net.NetworkCredential("3f61a514-0610-412e-9024-b4eb5670eb9d", "bb138472-93c1-4d47-91dc-a376e3c9dce2")
                //  , "smtp-server.embarkemail.com", 25);
                smtpClient.Deliver(message);
                // var objectToken = message.To;
                //m_SmtpClient.Send(message);//.Send(message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Problem with sending an email", ex);
            }
        }


        public void GenerateAndSendEmail(string recepient, MailParameters parameters)
        {

            var sendGridMail = SendGridMail.SendGrid.GetInstance();
            sendGridMail.From = new MailAddress(parameters.SenderEmail, parameters.SenderName);
            var mail = m_Container.Resolve<IMailBuilder>(parameters.MailResover);
            sendGridMail.EnableClickTracking();
            mail.GenerateMail(sendGridMail, parameters);

            sendGridMail.AddTo(recepient);
            var embarkeData = new Dictionary<string, string>() {
               { "embarkeAppId" , "3f61a514-0610-412e-9024-b4eb5670eb9d"},
               { "embarkeMsgId" , Guid.NewGuid().ToString() }
            };
            //var embarkeDataTimeWindow = new Dictionary<string, string>()
            //{
            //    { "X-EMBARKEAPI" , "{\"sendDate\": \"" +DateTime.UtcNow.ToString("o") + "\", \"maxSendHours\": 0}"

            //    }
            //};
            //sendGridMail.AddHeaders(embarkeDataTimeWindow);
            sendGridMail.AddUniqueIdentifiers(embarkeData);

            sendGridMail.EnableUnsubscribe("{unsubscribeUrl}");
            sendGridMail.AddSubVal("{email}", new List<string> { recepient });

            sendGridMail.EnableOpenTracking();
            sendGridMail.DisableGoogleAnalytics();
            Send(sendGridMail);

        }



    }
}
