using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Mail
{
    [UsedImplicitly]
    public class MailProvider : IMailProvider
    {
        private readonly HttpClient _client;

        public MailProvider(HttpClient restClient)
        {
            _client = restClient;
        }

        public async Task<bool> ValidateEmailAsync(string email, CancellationToken token)
        {
            var uri = new Uri("https://api.zerobounce.net/v2/validate");
            var nvc = new NameValueCollection()
            {
                ["api_key"] = "dece2a8c792848b39f7d7d888fd66193",
                ["email"] = email,
                ["ip_address"] = ""
            };

            using var c = new CancellationTokenSource(TimeSpan.FromSeconds(4));
            using var source = CancellationTokenSource.CreateLinkedTokenSource(token, c.Token);
            try
            {
                var uriBuilder = new UriBuilder(uri);
                uriBuilder.AddQuery(nvc);

                var result = await _client.GetAsync(uriBuilder.Uri, source.Token);
                result.EnsureSuccessStatusCode();
                using var s = await result.Content.ReadAsStreamAsync();
                var w = s.ToJsonReader<VerifyEmail>();

                var validStats = new[] { "valid", "catch-all" };
                if (validStats.Contains(w.Status, StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }
                return false;
            }
            catch (OperationCanceledException)
            {
                return true;
            }
        }


        private class VerifyEmail
        {
            // public string address { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }
            // public string sub_status { get; set; }
            //  public bool free_email { get; set; }
            // public object did_you_mean { get; set; }
            //  public object account { get; set; }
            // public object domain { get; set; }
            // public string domain_age_days { get; set; }
            // public string smtp_provider { get; set; }
            // public string mx_found { get; set; }
            //  public string mx_record { get; set; }
            // public string firstname { get; set; }
            // public string lastname { get; set; }
            //  public string gender { get; set; }
            // public object country { get; set; }
            // public object region { get; set; }
            //  public object city { get; set; }
            // public object zipcode { get; set; }
            //  public string processed_at { get; set; }
        }
    }
}
