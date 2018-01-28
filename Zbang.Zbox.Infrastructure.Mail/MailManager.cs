using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using SendGrid;
using SendGrid.Helpers.Mail;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public class MailManager2 : IMailComponent
    {
        private const string ApiKey = "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";
        private readonly ILifetimeScope _componentContent;
        private readonly ILogger _logger;

        public MailManager2(ILifetimeScope componentContent, ILogger logger)
        {
            _componentContent = componentContent;
            _logger = logger;
        }

        private static Task SendAsync(SendGridMessage message)
        {
            //var transport = new Web(new NetworkCredential(credentials.UserName, credentials.Password));
            //return transport.DeliverAsync(message);
            var client = new SendGridClient(ApiKey);
            return client.SendEmailAsync(message);
        }

        public async Task GenerateAndSendEmailAsync(string recipient, MailParameters parameters, CancellationToken cancellationToken = default(CancellationToken), string category = null)
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = parameters.UserCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(parameters.UserCulture.Name);

                var sendGridMail = new SendGridMessage
                {
                    From = new EmailAddress(parameters.SenderEmail, parameters.SenderName)
                };

                sendGridMail.AddTo(/*ConfigFetcher.IsEmulated ? "ram@cloudents.com" :*/ recipient);

                var mail = _componentContent.ResolveNamed<IMailBuilder>(parameters.MailResolver, new NamedParameter("parameters", parameters));
                sendGridMail.HtmlContent = mail.GenerateMail();
                sendGridMail.Subject = mail.AddSubject();
                sendGridMail.Categories = new List<string> { mail.AddCategory() };
                sendGridMail.SetGoogleAnalytics(true, mail.AddCategory());
                sendGridMail.SetSubscriptionTracking(true, substitutionTag: "{unsubscribeUrl}");
                sendGridMail.AddSubstitution("{email}",  recipient );
                sendGridMail.SetClickTracking(true,false);
                sendGridMail.SetOpenTracking(true,null);
                //sendGridMail.SetCategory(mail.AddCategory());
                //sendGridMail.EnableGoogleAnalytics("cloudentsMail", "email", null, campaign: mail.AddCategory());
                //sendGridMail.EnableUnsubscribe("{unsubscribeUrl}");
                //sendGridMail.AddSubstitution("{email}", new List<string> { recipient });
                //if (!string.IsNullOrEmpty(category))
                //{
                //    sendGridMail.SetCategory(category);
                //}
                //sendGridMail.EnableClickTracking();
                //sendGridMail.EnableOpenTracking();

                await SendAsync(sendGridMail).ConfigureAwait(false);
            }
            catch (FormatException ex)
            {
                _logger.Exception(ex, new Dictionary<string, string>
                {
                    ["recipient"] = recipient
                });
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, new Dictionary<string, string>
                {
                    ["recipient"] = recipient,
                    ["resolve"] = parameters.MailResolver,
                    ["type"] = parameters.GetType().Name
                });
            }
        }

        public Task<IEnumerable<string>> GetUnsubscribesAsync(DateTime startTime, int page, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEmailListFromApiCallAsync("suppression/unsubscribes", startTime, page);
        }

        public Task<IEnumerable<string>> GetBouncesAsync(DateTime startTime, int page, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEmailListFromApiCallAsync("suppression/bounces", startTime, page);
        }

        public Task<IEnumerable<string>> GetInvalidEmailsAsync(DateTime startTime, int page,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetEmailListFromApiCallAsync("suppression/invalid_emails", startTime, page);
        }

        //private const string ApiKey = "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";
        private static async Task<IEnumerable<string>> GetEmailListFromApiCallAsync(string requestUrl, DateTime startTime, int page)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var unixDateTime = (long)(startTime.ToUniversalTime() - epoch).TotalSeconds;

            var client = new SendGridClient(ApiKey);
            var result = await client.RequestAsync(SendGridClient.Method.GET, null,
                $"?limit={500}&offset={500 * page}&start_time={unixDateTime}", requestUrl);
            //var result = await client.Get($"{requestUrl}?limit={500}&offset={500 * page}&start_time={unixDateTime}").ConfigureAwait(false);
            
            var data = await result.Body.ReadAsStringAsync().ConfigureAwait(false);
            var emailArray = JArray.Parse(data);
            return emailArray.Select(s => s["email"].ToString());
        }

        public async Task DeleteUnsubscribeAsync(string email)
        {
            try
            {
                var client = new SendGridClient(ApiKey);
                await client.RequestAsync(SendGridClient.Method.DELETE, urlPath: $"/asm/suppressions/global/{email}").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Exception(ex);
            }
        }

        private const string MailGunApiKey = "key-5aea4c42085523a28a112c96d7b016d4";

        public Task SendSpanGunEmailAsync(string recipient,
            string ipPool,
            MailParameters parameters,
            int interVal,
            CancellationToken cancellationToken)
        {
            var mail = _componentContent.ResolveNamed<IMailBuilder>(parameters.MailResolver, new NamedParameter("parameters", parameters));

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
