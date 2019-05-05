using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Net;

namespace Cloudents.Web.Test.IntegrationTests
{
    [Collection(SbWebApplicationFactory.WebCollection)]
    public class UserApiTests //: IClassFixture<SbWebApplicationFactory>
    {
        //private readonly SbWebApplicationFactory _factory;
        private readonly System.Net.Http.HttpClient _client;

        public UserApiTests(SbWebApplicationFactory factory)
        {
            //_factory = factory;
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task Get_User()
        {
            await _client.LogInAsync();

            var response = await _client.GetAsync("api/course/search?term=fsdfds");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var courses = d["courses"]?.Value<JArray>();

            courses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAsync_Code()
        {
            var cred = new
            {
                email = "elad+99@cloudents.com",
                password = "123456789",
                fingerPrint = "string"
            };

            await _client.PostAsync("api/LogIn", HttpClient.CreateString(cred));

            var response = await _client.GetAsync("api/sms/code");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var code = d["code"]?.Value<string>();

            response.EnsureSuccessStatusCode();

            code.Should().NotBeNull();
        }

        [Fact]
        public async Task PostAsync_Sms()
        {
            var cred = new
            {
                confirmPassword = "123456789",
                email = "elad+99@cloudents.com",
                password = "123456789"
            };

            var phone = new
            {
                phoneNumber = "542473699",
                countryCode = 972
            };
            _client.DefaultRequestHeaders.Add("Referer", "swagger");

            await _client.PostAsync("api/Register", HttpClient.CreateString(cred));

            var response = await _client.PostAsync("api/Sms", HttpClient.CreateString(phone));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Sms_Resend()
        {
            var cred = new
            {
                confirmPassword = "123456789",
                email = "elad+99@cloudents.com",
                password = "123456789"
            };

            _client.DefaultRequestHeaders.Add("Referer", "swagger");

            await _client.PostAsync("api/Register", HttpClient.CreateString(cred));

            var response = await _client.PostAsync("api/Sms/resend", null);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PostAsync_Sms_Verify()
        {
            var cred = new
            {
                email = "elad+99@cloudents.com",
                password = "123456789",
                fingerPrint = "string"
            };

            var phone = new
            {
                number = "123456",
                fingerPrint = "string"
            };

            _client.DefaultRequestHeaders.Add("Referer", "swagger");

            await _client.PostAsync("api/Register", HttpClient.CreateString(cred));

            var response = await _client.PostAsync("api/Sms/verify", HttpClient.CreateString(phone));

            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task PostAsync_Resend_Email()
        {
            var cred = new
            {
                email = "dale@cloudents.com",
                password = "123456789",
                fingerPrint = "string"
            };

            await _client.PostAsync("api/Register", HttpClient.CreateString(cred));

            var response = await _client.PostAsync("api/Register/resend", HttpClient.CreateString("{}"));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAsync_Validate_Email()
        {
            var response = await _client.GetAsync("api/LogIn/ValidateEmail?email=elad%40cloudents.com");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAsync_Non_Validate_Email()
        {
            var response = await _client.GetAsync("api/LogIn/ValidateEmail?email=fsdfs%40cloudents.com");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var email = d["Email"]?.Value<JArray>();

            var error = d["Email"][0].Value<string>();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            error.Should().Be("Account cannot be found, please sign up");
        }
    }
}
