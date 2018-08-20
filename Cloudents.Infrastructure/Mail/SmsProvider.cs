using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Newtonsoft.Json;
//using Twilio;
//using Twilio.Rest.Lookups.V1;

namespace Cloudents.Infrastructure.Mail
{
    public class SmsProvider : ISmsProvider
    {
        private readonly IRestClient _restClient;


        private readonly string[] _badProviders = {
            "Twilio",
            "LEGOS - LOCAL EXCHANGE GLOBAL OPERATION SERVICES",
            "Belgacom Mobile - Proximus",
            "Tismi BV"
        };

        const string accountSid = "AC1796f09281da07ec03149db53b55db8d";
        const string authToken = "c4cdf14c4f6ca25c345c3600a72e8b49";

        public SmsProvider(IRestClient restClient)
        {
            _restClient = restClient;
            

            // TwilioClient.Init(accountSid, authToken);
        }

        public async Task<string> ValidateNumberAsync(string phoneNumber, CancellationToken token)
        {

            var uri = new Uri($"https://lookups.twilio.com/v1/PhoneNumbers/{phoneNumber}?Type=carrier");

            var byteArray = Encoding.ASCII.GetBytes($"{accountSid}:{authToken}");
            var authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var headers = new List<KeyValuePair<string, string>>();

            headers.Add(new KeyValuePair<string, string>("Authorization", authorization.ToString()));

            var result = await _restClient.GetAsync<PhoneValidator>(uri, null, headers, token);

            if (result == null)
            {
                return null;
            }
            var carrier = result.carrier;



            if (!string.Equals(carrier.type, "mobile", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }



            if (_badProviders.Contains(carrier.name, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

            return result.phone_number;
           


            // return result?.PhoneNumber;
        }




        public class PhoneValidator
        {
            public object caller_name { get; set; }
            public string country_code { get; set; }
            public string phone_number { get; set; }
            public string national_format { get; set; }
            public Carrier carrier { get; set; }
            public object add_ons { get; set; }
            public string url { get; set; }
        }

        public class Carrier
        {
            public string mobile_country_code { get; set; }
            public string mobile_network_code { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public object error_code { get; set; }
        }


    }
}