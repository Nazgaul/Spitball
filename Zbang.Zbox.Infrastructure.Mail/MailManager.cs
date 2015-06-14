using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private readonly IocFactory m_Container = IocFactory.IocWrapper;

        private Task SendAsync(ISendGrid message)
        {
            //try
            //{
            var transport = new Web(new NetworkCredential(SendGridUserName, SendGridPassword));
            return transport.DeliverAsync(message);
            //}
            //catch (Exception ex)
            //{
            //    TraceLog.WriteError("Problem with sending an email", ex);
            //}
        }


        public async Task GenerateAndSendEmailAsync(string recipient, MailParameters parameters)
        {
            try
            {
                //Thread.CurrentThread.CurrentUICulture = parameters.UserCulture;
                //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(parameters.UserCulture.Name);

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
                    {"embarkeAppId", "3f61a514-0610-412e-9024-b4eb5670eb9d"},
                    {"embarkeMsgId", Guid.NewGuid().ToString()}
                };

                sendGridMail.AddUniqueArgs(embarkeData);

                sendGridMail.EnableUnsubscribe("{unsubscribeUrl}");
                sendGridMail.AddSubstitution("{email}", new List<string> { recipient });

                sendGridMail.EnableClickTracking();
                sendGridMail.EnableOpenTracking();
                await SendAsync(sendGridMail);
            }
            catch (FormatException ex)
            {
                TraceLog.WriteError("recipient: " + recipient + " on trying to send mail", ex);
                throw;
            }

        }

        public async Task DeleteUnsubscribeAsync(string email)
        {
            try
            {
                // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                using (var client = new HttpClient())
                {

                    var content =
                        new StringContent(string.Empty);
                    await
                        client.PostAsync(
                            string.Format(
                                "https://sendgrid.com/api/unsubscribes.delete.json?api_user={0}&api_key={1}&email={2}",
                                SendGridUserName, SendGridPassword, email)
                            , content);
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on delete unsubscribe", ex);
            }
        }



        public async Task GenerateAndSendEmailAsync(IEnumerable<string> recipients, MailParameters parameters)
        {
            foreach (var recipient in recipients)
            {
                await GenerateAndSendEmailAsync(recipient, parameters);
            }
        }


        public Task GenerateAndSendEmailAsync(IEnumerable<string> recipients, string mailContent)
        {
            var sendGridMail = new SendGridMessage
            {
                From = new MailAddress("hatavotDb@cloudents.com"),
                To = recipients.Select(s => new MailAddress(s)).ToArray(),
                Text = mailContent,
                Subject = "Error in db",

            };
            return SendAsync(sendGridMail);
        }


       
    }
}
