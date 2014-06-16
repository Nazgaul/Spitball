﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Threading;
using SendGrid;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class MailManager2 : IMailComponent
    {
        private readonly IocFactory m_Container = IocFactory.Unity;

        private void Send(ISendGrid message)
        {
            try
            {
              
                //var smtpClient = SendGrid.Transport.SMTP.GetInstance(
                //    new System.Net.NetworkCredential("cloudents", "zbangitnow"), "smtp.sendgrid.net", 587);
                //var smtpClient = SendGridMail.Transport.SMTP.GetInstance(
                //  new System.Net.NetworkCredential("3f61a514-0610-412e-9024-b4eb5670eb9d", "bb138472-93c1-4d47-91dc-a376e3c9dce2")
                //  , "smtp-server.embarkemail.com", 25);
                var transport = new Web(new NetworkCredential("cloudents", "zbangitnow"));
                transport.Deliver(message);

                //smtpClient.Deliver(message);
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
            Thread.CurrentThread.CurrentUICulture = parameters.UserCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(parameters.UserCulture.Name);

            var sendGridMail = new SendGridMessage
            {
                From = new MailAddress(parameters.SenderEmail, parameters.SenderName)
            };

            //var sendGridMail = SendGridMail.SendGrid.GetInstance();
            var mail = m_Container.Resolve<IMailBuilder>(parameters.MailResover);
            sendGridMail.EnableClickTracking();
            mail.GenerateMail(sendGridMail, parameters);

            sendGridMail.AddTo(recepient);
            //sendGridMail.AddBcc("cloudents@outlook.com");
            //sendGridMail.AddTo("yaari.ram@gmail.com");
            //sendGridMail.AddTo(new List<string> { "dddavid2@gmail.com", "cloudents@outlook.com", "yaari_r@yahoo.com" });
            var embarkeData = new Dictionary<string, string>
            {
               { "embarkeAppId" , "3f61a514-0610-412e-9024-b4eb5670eb9d"},
               { "embarkeMsgId" , Guid.NewGuid().ToString() }
            };
            //var embarkeDataTimeWindow = new Dictionary<string, string>()
            //{
            //    { "X-EMBARKEAPI" , "{\"sendDate\": \"" +DateTime.UtcNow.ToString("o") + "\", \"maxSendHours\": 0}"

            //    }
            //};
            //sendGridMail.AddHeaders(embarkeDataTimeWindow);
            
            sendGridMail.AddUniqueArgs(embarkeData);

            sendGridMail.EnableUnsubscribe("{unsubscribeUrl}");
            sendGridMail.AddSubstitution("{email}", new List<string> { recepient });

            sendGridMail.EnableOpenTracking();
            sendGridMail.DisableGoogleAnalytics();
            Send(sendGridMail);

        }





        public void GenerateAndSendEmail(IEnumerable<string> recepients, MailParameters parameters)
        {
            foreach (var recepient in recepients)
            {
                GenerateAndSendEmail(recepient, parameters);
            }
        }
    }
}
