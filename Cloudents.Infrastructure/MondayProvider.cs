using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.Email;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure
{
    public class MondayProvider: IMondayProvider
    {
        private readonly HttpClient _client;
        private readonly string _credentials = "415bd8b82ac9702328cbe68a1f037079";
        private readonly string _userId = "8611308";
        private readonly string _groupId = "244704930";
        public MondayProvider(HttpClient client)
        {
            _client = client;
        }

        private async Task<int> CreateRecordAsync(string name, CancellationToken token)
        {
            var keyValue = new List<KeyValuePair<string, string>>();

            keyValue.Add(new KeyValuePair<string, string>("user_id", _userId));
            keyValue.Add(new KeyValuePair<string, string>("pulse[name]", name));
            keyValue.Add(new KeyValuePair<string, string>("group_id", _groupId));
            keyValue.Add(new KeyValuePair<string, string>("add_to_bottom", true.ToString()));
        

            using (var x = new FormUrlEncodedContent(keyValue))
            { 
                var t = $"https://api.monday.com:443/v1/boards/224787482/pulses.json?api_key={_credentials}";
                var uri = new Uri(t);
                
                HttpResponseMessage response = await _client.PostAsync(uri, x, token);

                if (!response.IsSuccessStatusCode)
                {
                    return -1;
                }
                var str = await response.Content.ReadAsStringAsync();

                var json = (JObject)JsonConvert.DeserializeObject(str);
                var id = json["pulse"]["id"].Value<int>();
                return id;
            }
        }

        private async Task UpdateTextRecordAsync(int pulseId, string columnId, string text, CancellationToken token)
        {
            var keyValue = new List<KeyValuePair<string, string>>();

            keyValue.Add(new KeyValuePair<string, string>("pulse_id", pulseId.ToString()));
            keyValue.Add(new KeyValuePair<string, string>("text", text));
            keyValue.Add(new KeyValuePair<string, string>("return_as_array", false.ToString()));

            using (var x = new FormUrlEncodedContent(keyValue))
            {
                var t = $"https://api.monday.com:443/v1/boards/224787482/columns/{columnId}/text.json?api_key={_credentials}";
                  
                var uri = new Uri(t);

                HttpResponseMessage response = await _client.PutAsync(uri, x, token);

                if (!response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"statusCode: {response.StatusCode} reason: {response.ReasonPhrase}, body: {str}");
                }
            }
        }

        private async Task UpdateDateRecordAsync(int pulseId, string columnId, CancellationToken token)
        {
            var keyValue = new List<KeyValuePair<string, string>>();

            keyValue.Add(new KeyValuePair<string, string>("pulse_id", pulseId.ToString()));
            keyValue.Add(new KeyValuePair<string, string>("date_str", DateTime.UtcNow.ToString("yyyy-MM-dd")));

            using (var x = new FormUrlEncodedContent(keyValue))
            {
                var t = $"https://api.monday.com:443/v1/boards/224787482/columns/{columnId}/date.json?api_key={_credentials}";

                var uri = new Uri(t);

                HttpResponseMessage response = await _client.PutAsync(uri, x, token);

                if (!response.IsSuccessStatusCode)
                {
                    var str = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"statusCode: {response.StatusCode} reason: {response.ReasonPhrase}, body: {str}");
                }
            }
        }

        public async Task CreateRecordAsync(RequestTutorEmail email, CancellationToken token)
        {
            var name = email.IsProduction ? email.Name : $"{email.Name} develop!";
            var id = await CreateRecordAsync(name, token);

            var phoneNumberTask = UpdateTextRecordAsync(id, "________________________", email.PhoneNumber, token);
            var subjectsTask = UpdateTextRecordAsync(id, "_____________1",
                string.Concat(email.Text, " ", email.Course), token);
            var universityTask = UpdateTextRecordAsync(id, "text9", email.University, token);
            var dateTask = UpdateDateRecordAsync(id, "date", token);


            await Task.WhenAll(phoneNumberTask, subjectsTask, universityTask, dateTask);
        }
    }
}
