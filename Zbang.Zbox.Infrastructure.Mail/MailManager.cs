using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SendGrid;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class MailManager2 : IMailComponent
    {

        private readonly IocFactory m_Container = IocFactory.IocWrapper;

        private static Task SendAsync(ISendGrid message, ICredentials credentials)
        {
            var transport = new Web(new NetworkCredential(credentials.UserName, credentials.Password));
            return transport.DeliverAsync(message);
        }


        public async Task GenerateAndSendEmailAsync(string recipient, MailParameters parameters, CancellationToken cancellationToken = default(CancellationToken), string category = null)
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = parameters.UserCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(parameters.UserCulture.Name);

                var sendGridMail = new SendGridMessage
                {
                    From = new MailAddress(parameters.SenderEmail, parameters.SenderName)
                };

                sendGridMail.AddTo(ConfigFetcher.IsEmulated ? "ram@cloudents.com" : recipient);

                var mail = m_Container.Resolve<IMailBuilder>(parameters.MailResover);
                mail.AddSubject(sendGridMail);
                mail.GenerateMail(sendGridMail, parameters);


                sendGridMail.EnableUnsubscribe("{unsubscribeUrl}");
                sendGridMail.AddSubstitution("{email}", new List<string> { recipient });
                if (!string.IsNullOrEmpty(category))
                {
                    
                    sendGridMail.SetCategory(category);
                }
                sendGridMail.EnableClickTracking();
                sendGridMail.EnableOpenTracking();
                await SendAsync(sendGridMail, new Credentials());
            }
            catch (FormatException ex)
            {
                TraceLog.WriteError("recipient: " + recipient + " on trying to send mail", ex);
                throw;
            }

        }

        public Task<IEnumerable<string>> GetUnsubscribesAsync(DateTime startTime, int page, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEmailListFromApiCallAsync("v3/suppression/unsubscribes", startTime, page, cancellationToken);
        }

        public Task<IEnumerable<string>> GetBouncesAsync(DateTime startTime, int page, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEmailListFromApiCallAsync("v3/suppression/bounces", startTime, page, cancellationToken);
        }

        public Task<IEnumerable<string>> GetInvalidEmailsAsync(DateTime startTime, int page,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEmailListFromApiCallAsync("v3/suppression/invalid_emails", startTime, page, cancellationToken);

        }

        private const string ApiKey = "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";
        //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
        private static async Task<IEnumerable<string>> GetEmailListFromApiCallAsync(string requestUrl, DateTime startTime, int page,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixDateTime = (long)(startTime.ToUniversalTime() - epoch).TotalSeconds;

            var client = new Client(ApiKey);
            var result = await client.Get($"{requestUrl}?limit={500}&offset={500 * page}&start_time={unixDateTime}");
            var data = await result.Content.ReadAsStringAsync();
            var emailArray = JArray.Parse(data);
            return emailArray.Select(s => s["email"].ToString());
        }

        public async Task DeleteUnsubscribeAsync(string email)
        {

            try
            {
                var client = new Client(ApiKey);
                var result = await client.GlobalSuppressions.Delete(email);
                TraceLog.WriteInfo($"result from delete unsubscribe {result.StatusCode}");
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on delete unsubscribe", ex);
            }
        }

        public async Task GenerateSystemEmailAsync(string subject, string text)
        {
            var sendGridMail = new SendGridMessage
            {
                From = new MailAddress("no-reply@spitball.co", "spitball system"),
                Text = text,
                Subject = $"{subject} {DateTime.UtcNow.ToShortDateString()}"
            };
            sendGridMail.AddTo("ram@cloudents.com");
            await SendAsync(sendGridMail, new Credentials());
        }

        public async Task SendSpanGunEmailAsync(string recipient, string ipPool,
            string body, string subject,
            string name, string category, string universityUrl)
        {
            var sendGridMail = new SendGridMessage
            {
                From = new MailAddress("michael@spitball.co", "Michael Baker")
            };
            

            sendGridMail.AddTo(ConfigFetcher.IsEmulated ? "ram@cloudents.com" : recipient);
            //sendGridMail.AddTo(ConfigFetcher.IsEmulated ? "jordan@spitball.co" : recipient);
            //sendGridMail.AddTo(ConfigFetcher.IsEmulated ? "shlomi@cloudents.com" : recipient);
            //sendGridMail.AddTo(ConfigFetcher.IsEmulated ? "eidan@cloudents.com" : recipient);

            var html = LoadMailTempate.LoadMailFromContentWithDot(new CultureInfo("en-US"), "Zbang.Zbox.Infrastructure.Mail.MailTemplate.SpamGun");
            html.Replace("{name}", name);
            html.Replace("{body}", body.Replace("\n", "<br>"));
            html.Replace("{email}", recipient);
            html.Replace("{uni_Url}", universityUrl);
            sendGridMail.Html = html.ToString();

            sendGridMail.EnableUnsubscribe("{unsubscribeUrl}");
            sendGridMail.SetCategory(category);
            sendGridMail.Subject = subject;
            sendGridMail.SetIpPool(ipPool);
            sendGridMail.EnableClickTracking();
            sendGridMail.EnableOpenTracking();
            await SendAsync(sendGridMail, new UsCredentials());

        }

        


    }
}
