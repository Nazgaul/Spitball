using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class SignTestsApi //: IClassFixture<SbWebApplicationFactory>
    {
        private readonly System.Net.Http.HttpClient _client;

        private readonly string _swaggerLink = "https://localhost/swagger";

        private readonly object _cred = new
        {
            confirmPassword = "123456789",
            email = "elad+99@cloudents.com",
            password = "123456789"
        };

        private readonly object _sms = new
        {
            number = "123456",
            fingerPrint = "string"
        };

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/register"
        };




        public SignTestsApi(SbWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
       _client.DefaultRequestHeaders.Referrer = new Uri(_swaggerLink);

        }

        [Fact]
        public async Task Post_Login_With_Email()
        {
            await _client.LogInAsync();
        }

        [Fact]
        public async Task Post_Register_With_Email()
        {
            var sign = new
            {
                email = "elad+99@cloudents.com",
                password = "123456789",
                confirmPassword = "123456789"
            };

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(sign));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Get_Sms_Code()
        {
            var responseObject = new
            {
                code = "972"
            };


            await _client.LogInAsync();

            _uri.Path = "api/sms/code";

            var response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            str.Should().Be(JsonConvert.SerializeObject(responseObject));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Post_Send_Sms_Code()
        {
            var sign = new
            {
                email = "elad+99@cloudents.com",
                password = "123456789",
                confirmPassword = "123456789"
            };
            
            var phone = new
            {
                phoneNumber = "542473699",
                countryCode = "972"
            };

            await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(sign));

            _uri.Path = "api/sms";

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(phone));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAsync_Code()
        {
            _uri.Path = "api/login";

            await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(_cred));

            _uri.Path = "api/sms/code";

            var response = await _client.GetAsync(_uri.Path);

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var code = d["code"]?.Value<string>();

            response.EnsureSuccessStatusCode();

            code.Should().NotBeNull();
        }

        [Fact]
        public async Task PostAsync_Sms_Resend()
        {
            _client.DefaultRequestHeaders.Add("Referer", "swagger");

            _uri.Path = "api/register";

            await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(_cred));

            _uri.Path = "api/sms/resend";

            var response = await _client.PostAsync(_uri.Path, null);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Sms_Verify()
        {
            _client.DefaultRequestHeaders.Add("Referer", "swagger");

            _uri.Path = "api/register";

            await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(_cred));

            _uri.Path = "api/sms/verify";

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateJsonString(_sms));

            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task PostAsync_Resend_Email()
        {
            await _client.PostAsync("api/Register", HttpClient.CreateJsonString(_cred));

            _uri.Path = "api/register/resend";

            var response = await _client.PostAsync(_uri.Path, HttpClient.CreateString("{}"));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAsync_Validate_Email()
        {
            _uri.Path = "api/login";

            var response = await _client.GetAsync(_uri.Path + "/ValidateEmail?email=elad%40cloudents.com");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAsync_Non_Validate_Email()
        {
            _uri.Path = "api/login";

            var response = await _client.GetAsync(_uri.Path + "/ValidateEmail?email=fsdfs%40cloudents.com");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            //var email = d["Email"]?.Value<JArray>();

            var error = d["Email"][0].Value<string>();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            error.Should().Be("Account cannot be found, please sign up");
        }
    }
}
