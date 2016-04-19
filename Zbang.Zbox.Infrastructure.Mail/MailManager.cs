using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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


        public async Task GenerateAndSendEmailAsync(string recipient, MailParameters parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = parameters.UserCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(parameters.UserCulture.Name);

                var sendGridMail = new SendGridMessage
                {
                    From = new MailAddress(parameters.SenderEmail, parameters.SenderName)
                };

                var mail = m_Container.Resolve<IMailBuilder>(parameters.MailResover);
                mail.AddSubject(sendGridMail);
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

        public async Task<IEnumerable<string>> GetUnsubscribesAsync(DateTime startTime, int page, CancellationToken cancellationToken = default(CancellationToken))
        {
            const string apiKey = "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";

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixDateTime = (long)(startTime.ToUniversalTime() - epoch).TotalSeconds;

            var client = new Client(apiKey);
            var result = await client.Get($"v3/suppression/unsubscribes?limit={500}&offset={500 * page}&start_time={unixDateTime}");
            var data = await result.Content.ReadAsStringAsync();
            var emailArray = JArray.Parse(data);
            return emailArray.Select(s => s["email"].ToString());


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
                            $"https://sendgrid.com/api/unsubscribes.delete.json?api_user={SendGridUserName}&api_key={SendGridPassword}&email={email}"
                            , content);
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on delete unsubscribe", ex);
            }
        }

        public async Task GenerateSystemEmailAsync(string text)
        {
            var sendGridMail = new SendGridMessage
            {
                From = new MailAddress("no-reply@spitball.co", "spitball system"),
                Text = text,
                Subject = "system notification"
            };

            //var mail = m_Container.Resolve<IMailBuilder>(parameters.MailResover);
            //mail.AddSubject(sendGridMail);
            //mail.GenerateMail(sendGridMail, parameters);



            //sendGridMail.AddTo("yaari.ram@gmail.com");
            sendGridMail.AddTo("ram@cloudents.com");
            await SendAsync(sendGridMail);
        }



        //public async Task GenerateAndSendEmailAsync(IEnumerable<string> recipients, MailParameters parameters)
        //{
        //    foreach (var recipient in recipients)
        //    {
        //        await GenerateAndSendEmailAsync(recipient, parameters);
        //    }
        //}



    }
}
