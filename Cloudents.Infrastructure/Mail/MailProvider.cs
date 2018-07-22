using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using Microsoft.Azure.Documents.SystemFunctions;
using Newtonsoft.Json;

namespace Cloudents.Infrastructure.Mail
{
    [UsedImplicitly]
    public class MailProvider : IMailProvider
    {

        private readonly Lazy<IRestClient> _restClient;

        public MailProvider(Lazy<IRestClient> restClient)
        {
            _restClient = restClient;
        }

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

        public async Task<bool> ValidateEmailAsync(string email, CancellationToken token)
        {
            var uri = new Uri($"https://api.mailgun.net/v3/address/validate");
            var nvc = new NameValueCollection()
            {
                ["address"] = email,
                ["api_key"] = "pubkey-871e78a663947b3b87c523a7b81c4b78",
                ["mailbox_verification"] = "true"
            };
            var w = await _restClient.Value.GetAsync<VerifyEmail>(uri, nvc, null, token).ConfigureAwait(false);

            return w?.IsValid ?? false;
        }


        public class VerifyEmail
        {
            //public string address { get; set; }
            //public object did_you_mean { get; set; }
            //public bool is_disposable_address { get; set; }
            //public bool is_role_address { get; set; }
            [JsonProperty("is_valid")]
            public bool IsValid { get; set; }
            //public string mailbox_verification { get; set; }
            //public Parts parts { get; set; }
            //public object reason { get; set; }
        }

        //public class Parts
        //{
        //    public object display_name { get; set; }
        //    public string domain { get; set; }
        //    public string local_part { get; set; }
        //}


    }
}
