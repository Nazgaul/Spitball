using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cloudents.Web.Test.IntegrationTests
{
    public class UserApiTests : IClassFixture<SbWebApplicationFactory>
    {
        private readonly SbWebApplicationFactory _factory;

        public UserApiTests(SbWebApplicationFactory factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task Get_User()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            var response = await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            response = await client.GetAsync("api/course/search?term=fsdfds");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var courses = d["courses"]?.Value<JArray>();

            courses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAsync_Code()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad+99@cloudents.com\",\"password\":\"123456789\",\"fingerPrint\":\"string\"}";

            await client.PostAsync("api/LogIn", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.GetAsync("api/sms/code");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var code = d["code"]?.Value<string>();

            response.StatusCode.Should().Be(200);

            code.Should().NotBeNull();
        }

        [Fact]
        public async Task PostAsync_Sms()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad+99@cloudents.com\",\"password\":\"123456789\",\"confirmPassword\":\"123456789\"}";

            string phone = "{\"phoneNumber\":\"542473699\",\"countryCode\":972}";

            client.DefaultRequestHeaders.Add("Referer", "swagger");

            await client.PostAsync("api/Register", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Sms", new StringContent(phone, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostAsync_Sms_Resend()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad+99@cloudents.com\",\"password\":\"123456789\",\"confirmPassword\":\"123456789\"}";

            client.DefaultRequestHeaders.Add("Referer", "swagger");

            await client.PostAsync("api/Register", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Sms/resend", null);

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostAsync_Sms_Verify()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"elad+99@cloudents.com\",\"password\":\"123456789\",\"confirmPassword\":\"123456789\"}";

            string phone = "{\"number\":\"123456\",\"fingerPrint\":\"string\"}";

            client.DefaultRequestHeaders.Add("Referer", "swagger");

            await client.PostAsync("api/Register", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Sms/verify", new StringContent(phone, Encoding.UTF8, "application/json"));

            response.StatusCode.Should().NotBe(500);
        }

        [Fact]
        public async Task PostAsync_Resend_Email()
        {
            var client = _factory.CreateClient();

            string cred = "{\"email\":\"dale@cloudents.com\",\"password\":\"123456789\",\"confirmPassword\":\"123456789\"}";

            await client.PostAsync("api/Register", new StringContent(cred, Encoding.UTF8, "application/json"));

            var response = await client.PostAsync("api/Register/resend", new StringContent("{}", Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task GetAsync_Validate_Email()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/LogIn/ValidateEmail?email=elad%40cloudents.com");

            response.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetAsync_Non_Validate_Email()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/LogIn/ValidateEmail?email=fsdfs%40cloudents.com");

            var str = await response.Content.ReadAsStringAsync();

            var d = JObject.Parse(str);

            var email = d["Email"]?.Value<JArray>();

            var error = d["Email"][0].Value<string>();

            response.StatusCode.Should().Be(400);
            error.Should().Be("Account cannot be found, please sign up");
        }
    }
}
