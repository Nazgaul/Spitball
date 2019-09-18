using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SmsApiTests
    {
        private readonly System.Net.Http.HttpClient _client;

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/Sms"
        };

        private readonly object user = new
        {
            confirmPassword = "123456789",
            email = "elad+99@cloudents.com",
            password = "123456789"
        };

        private readonly object sms = new
        {
            number = "123456",
            fingerPrint = "string"
        };

        private readonly object sign = new
        {
            email = "elad+99@cloudents.com",
            password = "123456789",
            confirmPassword = "123456789"
        };

        private readonly object responseObject = new
        {
            code = "972"
        };

        private readonly object phone = new
        {
            phoneNumber = "0542473699",
            countryCode = "972"
        };

        public SmsApiTests(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task GetAsync_Code()
        {
            _uri.Path = "api/login";

            await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(user));

            _uri.Path = "api/sms/code";

            var response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var code = d["code"]?.Value<string>();

            response.EnsureSuccessStatusCode();

            code.Should().NotBeNull();
        }

        [Fact]
        public async Task Get_Sms_Code()
        {
            await _client.LogInAsync();

            _uri.Path = "api/sms/code";

            var response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            str.Should().Be(JsonConvert.SerializeObject(responseObject));

            response.EnsureSuccessStatusCode();
        }

        [Fact(Skip ="Can not run this test more than once, need to delete phone from database first")]
        public async Task Post_Send_Sms_Code()
        {
            /*_uri.Path = "api/register";

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(user));*/
            
            _uri.Path = "api/sms";

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(phone));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Sms_Resend()
        {
            _uri.Path = "api/register";

            await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(user));

            _uri.Path = "api/sms/resend";

            var response = await _client.PostAsync(_uri.Path, null);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Sms_Verify()
        {
            _uri.Path = "api/register";

            await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(user));

            _uri.Path = "api/sms/verify";

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(sms));

            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
        }
    }
}
