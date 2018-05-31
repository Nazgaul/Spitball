using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Mail
{
    [UsedImplicitly]
    public class MailProvider : IMailProvider
    {
        private const string SendGridApiKey = "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";

        private readonly Lazy<IRestClient> _restClient;

        public MailProvider(Lazy<IRestClient> restClient)
        {
            _restClient = restClient;
        }

        //public Task GenerateSystemEmailAsync(string subject, string text, CancellationToken token)
        //{
        //    return SendEmailAsync("ram@cloudents.com", subject, text, token);
        //}

        //private static Task SendEmailAsync(string subject, string text, CancellationToken token)
        //{
        //    var client = new SendGridClient(SendGridApiKey);
        //    var msg = new SendGridMessage
        //    {
        //        From = new EmailAddress("no-reply@spitball.co", "spitball system"),
        //        Subject = subject,
        //        PlainTextContent = text,
        //        //HtmlContent = "<strong>and easy to do anywhere, even with C#</strong>"
        //    };
        //    msg.AddTo(new EmailAddress("ram@cloudents.com", "Ram Y"));
        //    return client.SendEmailAsync(msg, token);
        //}

        private const string MailGunApiKey = "key-5aea4c42085523a28a112c96d7b016d4";

        public async Task SendSpanGunEmailAsync(
            string ipPool,
            MailGunRequest parameters,
            CancellationToken cancellationToken)
        {
            var headers = new List<KeyValuePair<string, string>>();

            var byteArray = Encoding.ASCII.GetBytes($"api:{MailGunApiKey}");
            var authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            headers.Add(new KeyValuePair<string, string>("Authorization", authorization.ToString()));

            var uri = new Uri($"https://api.mailgun.net/v3/mg{ipPool}.spitball.co/messages");

            var serializedParams = parameters.GetType().GetProperties().Select(property =>
            {
                var key = property.Name;
                if (property.GetCustomAttribute(typeof(DataMemberAttribute)) is DataMemberAttribute att)
                {
                    key = att.Name;
                }

                var val = property.GetValue(parameters).ToString();
                return new KeyValuePair<string, string>(key, val);
            });

            using (var body = new FormUrlEncodedContent(serializedParams))
            {
                await _restClient.Value.PostAsync(uri, body, headers, cancellationToken).ConfigureAwait(false);
            }
        }

        //public Task SendEmailAsync(string email, string subject, string message, CancellationToken token)
        //{
        //    var client = new SendGridClient(SendGridApiKey);
        //    var msg = new SendGridMessage
        //    {
        //        From = new EmailAddress("no-reply@spitball.co", "spitball system"),
        //        Subject = subject,
        //        HtmlContent = message
        //    };
        //    msg.AddTo(new EmailAddress(email));
        //    return client.SendEmailAsync(msg, token);
        //}
    }
}
