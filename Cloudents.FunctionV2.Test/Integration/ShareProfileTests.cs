using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.FunctionsV2.Test.Integration
{
    public class ShareProfileTests
    {
        [Fact]
        public async Task CheckShareProfile()
        {
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
            var response = await client.GetAsync("https://spitball-function-dev2.azurewebsites.net/api/share/profile/638?width=50&height=50");
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var body = response.Content.ReadAsStringAsync();
            throw new ArgumentException($"response is {response.StatusCode} , body: {body}");
        }
    }
}