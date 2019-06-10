using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Net;
using System;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class UserApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        //private readonly SbWebApplicationFactory _factory;
        private readonly System.Net.Http.HttpClient _client;

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

        private readonly object _phone = new
        {
            phoneNumber = "542473699",
            countryCode = 972
        };

        private UriBuilder _uri = new UriBuilder()
        {
            Path = "api/course"
        };




        public UserApiTests(SbWebApplicationFactory factory)
        {
            //_factory = factory;
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task Get_User()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync(_uri.Path + "/search?term=fsdfds");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var courses = d["courses"]?.Value<JArray>();

            courses.Should().BeEmpty();
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
