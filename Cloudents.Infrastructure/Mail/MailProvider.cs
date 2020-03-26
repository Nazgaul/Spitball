using Cloudents.Core.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Mail
{
    public class MailProvider : IMailProvider
    {
        private readonly HttpClient _client;
        private const string SendGridApiKey = "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";

        public MailProvider(HttpClient restClient)
        {
            _client = restClient;
        }

        public async Task<bool> ValidateEmailAsync(string email, CancellationToken token)
        {

            var json = JsonConvert.SerializeObject(new { email }, new JsonSerializerSettings
            {
                //ContractResolver = ContractResolver,
                NullValueHandling = NullValueHandling.Ignore
            });
            using var sr = new StringContent(json, Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SendGridApiKey); //AuthenticationHeaderValue.Parse($"Bearer {SendGridApiKey}");
            var response = await _client.PostAsync("https://api.sendgrid.com/v3//validations/email", sr, token);

            if (!response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"statusCode: {response.StatusCode} reason: {response.ReasonPhrase}, body: {str}");
            }

            await using var s = await response.Content.ReadAsStreamAsync();


            var result = s.ToJsonReader<Rootobject>();

            return result.Result.Verdict == "Valid";
           
        }

        private class Rootobject
        {
            public Result Result { get;private set; }
        }

        private class Result
        {
           // public string email { get; set; }
            public string Verdict { get; private set; }
            public float Score { get; set; }
           // public string local { get; set; }
           // public string host { get; set; }
           // public Checks checks { get; set; }
          //  public string ip_address { get; set; }
        }

        //private class Checks
        //{
        //    public Domain domain { get; set; }
        //    public Local_Part local_part { get; set; }
        //    public Additional additional { get; set; }
        //}

        //private class Domain
        //{
        //    public bool has_valid_address_syntax { get; set; }
        //    public bool has_mx_or_a_record { get; set; }
        //    public bool is_suspected_disposable_address { get; set; }
        //}

        //private class Local_Part
        //{
        //    public bool is_suspected_role_address { get; set; }
        //}

        //private class Additional
        //{
        //    public bool has_known_bounces { get; set; }
        //    public bool has_suspected_bounces { get; set; }
        //}


       


        //private class VerifyEmail
        //{
        //    // public string address { get; set; }
        //    [JsonProperty("status")]
        //    public string Status { get; set; }
        //    // public string sub_status { get; set; }
        //    //  public bool free_email { get; set; }
        //    // public object did_you_mean { get; set; }
        //    //  public object account { get; set; }
        //    // public object domain { get; set; }
        //    // public string domain_age_days { get; set; }
        //    // public string smtp_provider { get; set; }
        //    // public string mx_found { get; set; }
        //    //  public string mx_record { get; set; }
        //    // public string firstname { get; set; }
        //    // public string lastname { get; set; }
        //    //  public string gender { get; set; }
        //    // public object country { get; set; }
        //    // public object region { get; set; }
        //    //  public object city { get; set; }
        //    // public object zipcode { get; set; }
        //    //  public string processed_at { get; set; }
        //}
    }
}
