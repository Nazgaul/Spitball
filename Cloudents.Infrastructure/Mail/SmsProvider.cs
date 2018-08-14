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
using Twilio;
using Twilio.Rest.Lookups.V1;

namespace Cloudents.Infrastructure.Mail
{
    public class SmsProvider : ISmsProvider
    {

        public SmsProvider()
        {
            const string accountSid = "AC1796f09281da07ec03149db53b55db8d";
            const string authToken = "c4cdf14c4f6ca25c345c3600a72e8b49";

            TwilioClient.Init(accountSid, authToken);
        }

        public async Task<string> ValidateNumberAsync(string phoneNumber, CancellationToken token)
        {

            var type = new List<string> {
                "carrier"
            };

            try
            {
                var phoneNumberResult = await PhoneNumberResource.FetchAsync(
                    type: type,
                    pathPhoneNumber: new Twilio.Types.PhoneNumber(phoneNumber)
                );
                var carrier = phoneNumberResult.Carrier;
                if (carrier.TryGetValue("type", out var val))
                {
                    if (!string.Equals(val, "mobile", StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }

                if (carrier.TryGetValue("name", out var val2))
                {
                    var badProviders = new[]
                    {
                        "Twilio",
                        "LEGOS - LOCAL EXCHANGE GLOBAL OPERATION SERVICES",
                        "Belgacom Mobile - Proximus",
                        "Tismi BV"
                    };
                    if (badProviders.Contains(val2, StringComparer.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }

                return phoneNumberResult.PhoneNumber.ToString();
            }
            catch (Twilio.Exceptions.ApiException e)
            {
                return null;
            }
            //var uri = new Uri($"https://lookups.twilio.com/v1/PhoneNumbers/{phoneNumber}");

            //var byteArray = Encoding.ASCII.GetBytes($"AC1796f09281da07ec03149db53b55db8d:c4cdf14c4f6ca25c345c3600a72e8b49");
            //var authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            //var headers = new List<KeyValuePair<string, string>>();

            //headers.Add(new KeyValuePair<string, string>("Authorization", authorization.ToString()));

            //var result = await _client.GetAsync<PhoneValidator>(uri, null, headers, token);

            // return result?.PhoneNumber;
        }

        public class PhoneValidator
        {
            // public object caller_name { get; set; }
            // public string country_code { get; set; }
            [JsonProperty("phone_number")]
            public string PhoneNumber { get; set; }
            // public string national_format { get; set; }
            // public object carrier { get; set; }
            // public object add_ons { get; set; }
            // public string url { get; set; }
        }
    }
}