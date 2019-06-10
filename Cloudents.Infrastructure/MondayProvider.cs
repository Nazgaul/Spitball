using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure
{
    public class MondayProvider: IMondayProvider
    {
        private readonly HttpClient _client;
        private const string Credentials = "415bd8b82ac9702328cbe68a1f037079";
        private const string UserId = "8611308";
        private const string GroupId = "244704930";
        private readonly ILogger _logger;
        public MondayProvider(HttpClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        private async Task<int> CreateRecordAsync(string name, CancellationToken token)
        {
            var keyValue = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("user_id", UserId),
                new KeyValuePair<string, string>("pulse[name]", name),
                new KeyValuePair<string, string>("group_id", GroupId),
                new KeyValuePair<string, string>("add_to_bottom", bool.TrueString)
            };



            using (var formBodyContent = new FormUrlEncodedContent(keyValue))
            {
                var url = $"https://api.monday.com:443/v1/boards/224787482/pulses.json?api_key={Credentials}";

                using (var response = await _client.PostAsync(url, formBodyContent, token))
                {
                    response.EnsureSuccessStatusCode();
                    var str = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(str);
                    var id = json["pulse"]["id"].Value<int>();
                    return id;
                }

               
            }
        }

        private async Task UpdateTextRecordAsync(int pulseId, string columnId, string text, CancellationToken token)
        {
            var keyValue = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("pulse_id", pulseId.ToString()),
                new KeyValuePair<string, string>("text", text),
                new KeyValuePair<string, string>("return_as_array", false.ToString())
            };


            using (var x = new FormUrlEncodedContent(keyValue))
            {
                var uri = $"https://api.monday.com:443/v1/boards/224787482/columns/{columnId}/text.json?api_key={Credentials}";


                using (var response = await _client.PutAsync(uri, x, token))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var str = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"statusCode: {response.StatusCode} reason: {response.ReasonPhrase}, body: {str}");
                    }
                }
            }
        }

        private async Task UpdateDateRecordAsync(int pulseId, string columnId, CancellationToken token)
        {
            var keyValue = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("pulse_id", pulseId.ToString()),
                new KeyValuePair<string, string>("date_str", DateTime.UtcNow.ToString("yyyy-MM-dd"))
            };


            using (var x = new FormUrlEncodedContent(keyValue))
            {
                var uri = $"https://api.monday.com:443/v1/boards/224787482/columns/{columnId}/date.json?api_key={Credentials}";

                using (var response = await _client.PutAsync(uri, x, token))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var str = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"statusCode: {response.StatusCode} reason: {response.ReasonPhrase}, body: {str}");
                    }
                }
            }
        }

        public async Task CreateRecordAsync(RequestTutorEmail email, CancellationToken token)
        {
            try
            {
                var name = email.IsProduction ? email.Name : $"{email.Name} develop!";
                var id = await CreateRecordAsync(name, token);

                var refererArr = email.Referer.Split('&');
                var utmString = refererArr.FirstOrDefault(w => w.Contains("utm_source"));
                string utm = string.Empty;
                if (!string.IsNullOrEmpty(utmString))
                {
                    utm = utmString.Split('=').LastOrDefault();
                }

                var utmTask = UpdateTextRecordAsync(id, "text38", utm, token);
                var phoneNumberTask = UpdateTextRecordAsync(id, "__________7", email.PhoneNumber, token);
                var subjectsTask = UpdateTextRecordAsync(id, "_____________1",
                    string.Concat(email.Text, " ", email.Course), token);
                var universityTask = UpdateTextRecordAsync(id, "text9", email.University, token);
                var dateTask = UpdateDateRecordAsync(id, "date", token);


                await Task.WhenAll(utmTask, phoneNumberTask, subjectsTask, universityTask, dateTask);
            }
            catch (Exception e)
            {
                _logger.Exception(e,new Dictionary<string, string>()
                {
                    ["provider"] = "Monday"
                });
            }
        }
    }
}
