using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using SendGrid;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class MailManager2 : IMailComponent
    {
        private readonly ILifetimeScope m_ComponentContent;
        private readonly ILogger m_Logger;

        public MailManager2(ILifetimeScope componentContent, ILogger logger)
        {
            m_ComponentContent = componentContent;
            m_Logger = logger;
        }

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

                var mail = m_ComponentContent.ResolveNamed<IMailBuilder>(parameters.MailResover, new NamedParameter("parameters", parameters));
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
                m_Logger.Exception(ex, new Dictionary<string, string>
                {
                    ["recipient"] = recipient
                });
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex, new Dictionary<string, string>
                {
                    ["recipient"] = recipient,
                    ["resolve"] = parameters.MailResover,
                    ["type"] = parameters.GetType().Name
                });
            }
        }

        public Task<IEnumerable<string>> GetUnsubscribesAsync(DateTime startTime, int page, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEmailListFromApiCallAsync("v3/suppression/unsubscribes", startTime, page);
        }

        public Task<IEnumerable<string>> GetBouncesAsync(DateTime startTime, int page, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEmailListFromApiCallAsync("v3/suppression/bounces", startTime, page);
        }

        public Task<IEnumerable<string>> GetInvalidEmailsAsync(DateTime startTime, int page,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEmailListFromApiCallAsync("v3/suppression/invalid_emails", startTime, page);
        }

        private const string ApiKey = "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";
        private static async Task<IEnumerable<string>> GetEmailListFromApiCallAsync(string requestUrl, DateTime startTime, int page)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixDateTime = (long)(startTime.ToUniversalTime() - epoch).TotalSeconds;

            var client = new Client(ApiKey);
            var result = await client.Get($"{requestUrl}?limit={500}&offset={500 * page}&start_time={unixDateTime}").ConfigureAwait(false);
            var data = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            var emailArray = JArray.Parse(data);
            return emailArray.Select(s => s["email"].ToString());
        }

        public async Task DeleteUnsubscribeAsync(string email)
        {
            try
            {
                var client = new Client(ApiKey);
                await client.GlobalSuppressions.Delete(email).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                m_Logger.Exception(ex);
            }
        }

        public Task GenerateSystemEmailAsync(string subject, string text, string to = "ram@cloudents.com")
        {
            var sendGridMail = new SendGridMessage
            {
                From = new MailAddress("no-reply@spitball.co", "spitball system"),
                Text = text,
                Subject = $"{subject} {DateTime.UtcNow.ToShortDateString()}"
            };
            sendGridMail.AddTo(to);
            return SendAsync(sendGridMail, new Credentials());
        }

        private const string MailGunApiKey = "key-5aea4c42085523a28a112c96d7b016d4";

        public Task SendSpanGunEmailAsync(string recipient,
            string ipPool,
            MailParameters parameters,
            int interVal,
            CancellationToken cancellationToken)
        {
            var mail = m_ComponentContent.ResolveNamed<IMailBuilder>(parameters.MailResover, new NamedParameter("parameters", parameters));

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
