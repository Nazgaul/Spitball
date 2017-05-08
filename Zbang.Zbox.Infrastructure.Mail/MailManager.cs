using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
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

                sendGridMail.AddTo(/*ConfigFetcher.IsEmulated ? "ram@cloudents.com" :*/ recipient);

                var mail = m_Container.Resolve<IMailBuilder>(parameters.MailResover, new IocParameterOverride("parameters", parameters));
                sendGridMail.Html = mail.GenerateMail();
                sendGridMail.Subject = mail.AddSubject();
                sendGridMail.SetCategory(mail.AddCategory());
                sendGridMail.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: mail.AddCategory());

                sendGridMail.EnableUnsubscribe("{unsubscribeUrl}");
                sendGridMail.AddSubstitution("{email}", new List<string> { recipient });
                if (!string.IsNullOrEmpty(category))
                {
                    sendGridMail.SetCategory(category);
                }
                sendGridMail.EnableClickTracking();
                sendGridMail.EnableOpenTracking();

                await SendAsync(sendGridMail, new Credentials()).ConfigureAwait(false);
            }
            catch (FormatException ex)
            {
                TraceLog.WriteError("recipient: " + recipient + " on trying to send mail", ex);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("recipient: " + recipient + " on trying to send mail " + "resolve:" + parameters.MailResover + "type: " + parameters.GetType().Name, ex);
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
                await client.GlobalSuppressions.Delete(email);
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


        private const string MailGunApiKey = "key-5aea4c42085523a28a112c96d7b016d4";

        public Task SendSpanGunEmailAsync(string recipient,
            string ipPool,
            MailParameters parameters,
            int interVal,
            CancellationToken cancellationToken)
        {

            var mail = m_Container.Resolve<IMailBuilder>(parameters.MailResover, new IocParameterOverride("parameters", parameters));

            var client = new RestClient
            {
                BaseUrl = new Uri("https://api.mailgun.net/v3"),
                Authenticator = new HttpBasicAuthenticator("api",
                   MailGunApiKey)
            };
            var request = new RestRequest
            {
                Resource = "{domain}/messages",
                Method = Method.POST
            };
            request.AddParameter("domain",
                                 $"mg{ipPool}.spitball.co", ParameterType.UrlSegment);

            request.AddParameter("from", parameters.SenderName);
            request.AddParameter("to", ConfigFetcher.IsEmulated ? "ram@cloudents.com" : recipient);
            //request.AddParameter("bcc", "sbtester82.5@gmail.com");
            request.AddParameter("subject", mail.AddSubject());
            request.AddParameter("html", mail.GenerateMail().Replace("{email}", recipient));
            request.AddParameter("o:tag", mail.AddCategory());
            request.AddParameter("o:campaign", "spamgun");
            request.AddParameter("o:deliverytime",
                DateTime.UtcNow.AddMinutes(interVal).ToString("ddd, dd MMM yyyy HH:mm:ss UTC"));

            return client.ExecuteTaskAsync(request, cancellationToken);
        }
    }
}
