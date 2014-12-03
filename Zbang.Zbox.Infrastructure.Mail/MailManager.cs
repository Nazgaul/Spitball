using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using SendGrid;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class MailManager2 : IMailComponent
    {
        private const string SendGridUserName = "cloudents";
        private const string SendGridPassword = "zbangitnow";
        private readonly IocFactory m_Container = IocFactory.Unity;

        private void Send(ISendGrid message)
        {
            try
            {
                var transport = new Web(new NetworkCredential(SendGridUserName, SendGridPassword));
                transport.Deliver(message);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("Problem with sending an email", ex);
            }
        }


        public void GenerateAndSendEmail(string recipient, MailParameters parameters)
        {
            Thread.CurrentThread.CurrentUICulture = parameters.UserCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(parameters.UserCulture.Name);

            var sendGridMail = new SendGridMessage
            {
                From = new MailAddress(parameters.SenderEmail, parameters.SenderName)
            };

            var mail = m_Container.Resolve<IMailBuilder>(parameters.MailResover);
            
            mail.GenerateMail(sendGridMail, parameters);

            //sendGridMail.AddTo("yaari.ram@gmail.com");
            sendGridMail.AddTo(recipient);
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
            sendGridMail.AddSubstitution("{email}", new List<string> { recipient });

            sendGridMail.EnableClickTracking();
            sendGridMail.EnableOpenTracking();
            Send(sendGridMail);

        }

        public async Task DeleteUnsubscribe(string email)
        {
            using (var client = new HttpClient())
            {

                var content =
                    new StringContent(string.Empty);
                await client.PostAsync(string.Format("https://sendgrid.com/api/unsubscribes.delete.json?api_user={0}&api_key={1}&email={2}",SendGridUserName,SendGridPassword,email)
                    ,content);
            }
        }



        public void GenerateAndSendEmail(IEnumerable<string> recipients, MailParameters parameters)
        {
            foreach (var recipient in recipients)
            {
                GenerateAndSendEmail(recipient, parameters);
            }
        }
    }
}
